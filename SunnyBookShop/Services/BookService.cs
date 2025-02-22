using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;
using SunnyBookShop.Utils;
using SunnyBookShop.ViewModels;

namespace SunnyBookShop.Services;

public interface IBookService
{
    Task<HomeViewModel> GetHomeBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<List<Book>> GetBooksByCategoryAsync(string category);
    Task<List<Book>> GetBooksBySubCategoryAsync(string subCategory);
    List<Book> GetSortedBooks(List<Book> books, SortState sortOrder);

    Task<List<Book>> FindBooksAsync(string searchString);
    Task<BookDetailsViewModel> GetBookDetailsAsync(int id, string userId);
    Task AddToCartAsync(CartItem cartItem);
    Task DeleteFromCartAsync(string userId, int bookId);
    Task<bool> AddCommentAsync(Review review);
}

public class BookService: IBookService
{
    private readonly ApplicationDbContext _dbContext;
    
    public BookService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<HomeViewModel> GetHomeBooksAsync()
    {
        var books = _dbContext.Books.Include(r => r.Reviews);
        var newBooks = await books.OrderByDescending(b => b.CreatedAt).Take(6).ToListAsync();
        var bestBooks = await books.OrderByDescending(b => b.Reviews.Count).Take(6).ToListAsync();
        var cheapBooks = await books.OrderBy(b => b.Price).Take(6).ToListAsync();

        return new HomeViewModel
        {
            NewBooks = newBooks,
            BestBooks = bestBooks,
            CheapBooks = cheapBooks
        };
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Book>> FindBooksAsync(string searchString)
    {
        var books = await _dbContext.Books
            .Where(r => 
                r.Title.Contains(searchString) 
                || r.Author.Contains(searchString))
            .ToListAsync();
        return books;
    }

    public async Task<List<Book>> GetBooksByCategoryAsync(string category)
    {
        var books = await _dbContext.Books
            .Where(c => c.Category == category)
            .ToListAsync();
        return books;
    }

    public async Task<List<Book>> GetBooksBySubCategoryAsync(string subCategory)
    {
        var books = await _dbContext.Books
            .Where(r => r.SubCategory == subCategory)
            .ToListAsync();
        return books;
    }

    public List<Book> GetSortedBooks(List<Book> books, SortState sortOrder)
    {
        List<Book> sortedBooks = books;
        
        if (books.Any())
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    sortedBooks = books.OrderByDescending(b => b.Title).ToList();
                    break;
                case SortState.AuthorAsc:
                    sortedBooks = books.OrderBy(b => b.Author).ToList();
                    break;
                case SortState.AuthorDesc:
                    sortedBooks = books.OrderByDescending(b => b.Author).ToList();
                    break;
                case SortState.PriceAsc:
                    sortedBooks = books.OrderBy(b => b.Price).ToList();
                    break;
                case SortState.PriceDesc:
                    sortedBooks = books.OrderByDescending(b => b.Price).ToList();
                    break;
                default:
                    sortedBooks = books.OrderBy(b => b.Title).ToList();
                    break;
            }
        }

        return sortedBooks.Any() ? sortedBooks : books;
    }

    public async Task<BookDetailsViewModel> GetBookDetailsAsync(int id, string userId)
    {
        User? user = null;
        if (!string.IsNullOrEmpty(userId))
            user = await _dbContext.Users
                .Include(u => u.Profile)
                .FirstAsync(u => u.Id == int.Parse(userId));
        
        Book? book = await _dbContext.Books
            .Include(b => b.Reviews)
            .ThenInclude(r => r.User)
            .ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(b => b.Id == id);
        
        var reviewsCount = book?.Reviews.Count;
        var positiveReviews = 0;
        var positiveReviewsPercents = 0;
        if (reviewsCount > 0)
        {
            positiveReviews = book!.Reviews.Where(r => r.Regard == "Positive").Count();
            positiveReviewsPercents = (positiveReviews / reviewsCount) * 100 ?? 0;
        }

        if (book is not null)
        {
            var similarBooks = await _dbContext.Books.Where(b => b.Author == book.Author)
                .OrderByDescending(b => b.CreatedAt)
                .Take(6)
                .ToListAsync();
            BookDetailsViewModel detailsVM = new()
            {
                Book = book,
                User = user,
                ReviewsCount = reviewsCount,
                PositiveReviews = positiveReviews,
                PositiveReviewsPercents = positiveReviewsPercents,
                SimilarBooks = similarBooks
            };
            return detailsVM;
        }
        return null;
    }

    public async Task AddToCartAsync(CartItem cartItem)
    {
        await _dbContext.CartItems.AddAsync(cartItem); 
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteFromCartAsync(string userId, int id)
    {
        var cartItem = await _dbContext.CartItems
            .Include(u => u.User)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (cartItem.UserId == Int32.Parse(userId))
        {
            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<bool> AddCommentAsync(Review review)
    {
        await _dbContext.Reviews.AddAsync(review);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }
}
using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;
using SunnyBookShop.ViewModels;

namespace SunnyBookShop.Services;

public interface IBookService
{
    Task<HomeViewModel> GetBooksAsync();
    Task<List<Book>> FindBookAsync(string searchString);
    Task<BookDetailsViewModel> GetBookDetailsAsync(int id, string userId);
    Task AddToCartAsync(CartItem cartItem);
    Task DeleteFromCartAsync(string userId, int bookId);
}

public class BookService: IBookService
{
    private readonly ApplicationDbContext _dbContext;
    
    public BookService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<HomeViewModel> GetBooksAsync()
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

    public async Task<List<Book>> FindBookAsync(string searchString)
    {
        var books = await _dbContext.Books
            .Where(r => r.Title
                .Contains(searchString) || r.Author.Contains(searchString))
            .ToListAsync();
        return books;
    }

    public async Task<BookDetailsViewModel> GetBookDetailsAsync(int id, string userId)
    {
        User? user = null;
        if (!string.IsNullOrEmpty(userId))
            user = await _dbContext.Users.Include(u => u.Profile).FirstAsync(u => u.Id == int.Parse(userId));
        Book? book = await _dbContext.Books.Include(b => b.Reviews).ThenInclude(r => r.User)
            .ThenInclude(u => u.Profile).FirstOrDefaultAsync(b => b.Id == id);
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
                .OrderByDescending(b => b.CreatedAt).Take(6).ToListAsync();
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
}
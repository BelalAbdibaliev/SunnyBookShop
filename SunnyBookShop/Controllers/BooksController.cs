using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyBookShop.Models;
using SunnyBookShop.Services;
using SunnyBookShop.Utils;
using SunnyBookShop.ViewModels;

namespace SunnyBookShop.Controllers;

public class BooksController: Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }
    
    public async Task<IActionResult> Index(string? category, string? subCategory,
        string? searchString, int page = 1, SortState sortOrder = SortState.NameAsc)
    {
        int pageSize = 18;
        List<Book> books = new List<Book>();

        if (!string.IsNullOrEmpty(category))
            books = await _bookService.GetBooksByCategoryAsync(category);
        ViewBag.Category = category;

        if (books.Any() && !string.IsNullOrEmpty(subCategory))
        {
            subCategory = subCategory.Replace("+", " ");
            books = await _bookService.GetBooksBySubCategoryAsync(subCategory);
            ViewBag.SubCategory = subCategory;
        }

        if (books.Any() && !string.IsNullOrEmpty(searchString))
            books = await _bookService.FindBooksAsync(searchString);
        
        if (books.Any())
            books = _bookService.GetSortedBooks(books, sortOrder);

        var count = books.Count;
        var items = books.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        BookIndexViewModel bookIndexVM = new(
            items,
            new PageViewModel(count, page, pageSize),
            new SortViewModel(sortOrder)
        );

        return View(bookIndexVM);
    }

    public async Task<IActionResult> Details(int id)
    {
        string? userId = User.FindFirst("UserId")?.Value;
        var result = await _bookService.GetBookDetailsAsync(id, userId);
        
        return View(result);
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(
        [Bind("Book", "BookId", "User", "UserId", "Quantity")]
        CartItem cartItem)
    {
        await _bookService.AddToCartAsync(cartItem);
        
        return RedirectToAction("Cart", "User");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFromCart(int id)
    {
        var userId = User.FindFirst("UserId")?.Value;
        await _bookService.DeleteFromCartAsync(userId, id);
        
        return RedirectToAction("Cart", "User");
    }
}
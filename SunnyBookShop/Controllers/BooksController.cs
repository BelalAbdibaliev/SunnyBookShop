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
        return View();
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
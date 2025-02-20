using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyBookShop.Models;
using SunnyBookShop.Services;

namespace SunnyBookShop.Controllers;

public class AdminController: Controller
{
    private readonly IAdminService _adminService;
    private readonly IBookService _bookService;

    public AdminController(IAdminService adminService, IBookService bookService)
    {
        _adminService = adminService;
        _bookService = bookService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddBook()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBook(Book book)
    {
        if (!ModelState.IsValid)
            return View(book);
        
        var result = await _adminService.AddBookAsync(book);
        if(result is null)
            return BadRequest();
        
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book is null)
            return NotFound();
        
        return View(book);
    }

    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> EditBook(int id, [Bind("Id", "Title", "Author", "Publisher",
            "Price", "Count", "Category", "SubCategory", "PosterUrl", "PosterFile", "Description")]
        Book book)
    {
        if (id != book.Id)
            return NotFound();
        if (!ModelState.IsValid)
            return View(book);
        
        var result = await _adminService.EditBookAsync(id, book);
        if(result is null)
            return BadRequest();
        
        return RedirectToAction("Details", "Books", new { id = book.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var isDeleted = await _adminService.DeleteBookAsync(id);
        if(!isDeleted)
        {
            TempData["ErrorMessage"] = "Something is wrong!";
            return BadRequest();
        }
        
        return RedirectToAction("Index", "Home");
    }
}
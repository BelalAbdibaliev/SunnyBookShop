using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyBookShop.Models;
using SunnyBookShop.Services;

namespace SunnyBookShop.Controllers;

public class AdminController: Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
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
}
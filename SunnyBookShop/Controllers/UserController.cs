using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyBookShop.Services;

namespace SunnyBookShop.Controllers;

public class UserController: Controller
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Authorize]
    public async Task<IActionResult> Cart()
    {
        string? userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }
        var cartItems = await _userService.GetCartItems(userId);
        return View(cartItems);
    }
}
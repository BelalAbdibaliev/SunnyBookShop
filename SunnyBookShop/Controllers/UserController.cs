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
    public async Task<IActionResult> Profile(int id)
    {
        var profileVM = await _userService.GetUserProfileAsync(id);
        
        if(profileVM == null)
            return NotFound();
        
        return View(profileVM);
    }
    
    [Authorize]
    public async Task<IActionResult> Cart()
    {
        string? userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }
        var cartItems = await _userService.GetCartItemsAsync(userId);
        return View(cartItems);
    }
}
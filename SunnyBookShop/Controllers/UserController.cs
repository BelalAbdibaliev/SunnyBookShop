using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunnyBookShop.Models;
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
        var profileVM = await _userService.GetUserAsync(id);
        
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
    
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        int? userId = int.Parse(User.FindFirst("UserId")?.Value);
            
        var checkoutVM = await _userService.GetCheckoutViewModelAsync(userId);

        if(checkoutVM == null)
            return NotFound();
            
        return View(checkoutVM);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Checkout(decimal totalPrice)
    {
        string? userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest();

        var result =  await _userService.CheckOut(Int32.Parse(userId), totalPrice);
        if(result is null)
            return BadRequest();
        
        return RedirectToAction("Profile", "User", new { Id = int.Parse(userId) });
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var userProfile = await _userService.GetUserProfileAsync(id);
        if(userProfile is null)
        {
            return NotFound();
        }
        
        return View(userProfile);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [Bind("Name", "Location", "PhoneNumber", "AvatarFile")]
        UserProfile profile)
    {
        if (!ModelState.IsValid)
            return View(profile);

        int userId = int.Parse(User.FindFirst("UserId")?.Value!);
        var result = await _userService.EditUserProfileAsync(userId, profile);
        
        if(!result)
            return BadRequest();
        
        return RedirectToAction("Profile", new { Id = userId });
    }
    
    [Authorize]
    public async Task<IActionResult> PurchaseHistory()
    {
        int userId = int.Parse(User.FindFirst("UserId")?.Value!);

        var orders = await _userService.GetPurchasesHistoryAsync(userId);

        return View(orders);
    }
}
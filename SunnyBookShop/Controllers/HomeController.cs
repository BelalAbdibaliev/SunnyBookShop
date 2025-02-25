using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;
using SunnyBookShop.Services;
using SunnyBookShop.ViewModels;

namespace SunnyBookShop.Controllers;

public class HomeController : Controller
{
    const string authScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    
    private readonly IBookService _bookService;
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(IBookService bookService, IUserService userService, ApplicationDbContext dbContext)
    {
        _bookService = bookService;
        _userService = userService;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetHomeBooksAsync();
        
        return View(books);
    }
    
    public IActionResult Login()
    {
        if (User.Identity is not null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
        }

        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel inputData, string? ReturnUrl)
    {
        User? user = await _userService.FindByEmailAsync(inputData.Email);
        if (user is null)
        {
            ModelState.AddModelError(inputData.Email, "Invalid Email or Password Address");
            return View(inputData);
        }
        
        var result = await _userService.Login(user, inputData.Password, HttpContext);
        if(!result)
            return View(inputData);

        return LocalRedirect(ReturnUrl ?? "/");
    }
    
    public IActionResult SignUp()
    {
        if (User.Identity is not null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(
        [Bind("Email", "Password", "ConfirmPassword", "TermsConditions")] User user)
    {
        if (!ModelState.IsValid)
            return View(user);

        var result = await _userService.SignUp(user);
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            return View(user);
        }
        await HttpContext.SignInAsync(authScheme, result.Value);
        return LocalRedirect("/");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(authScheme);
        return LocalRedirect("/");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
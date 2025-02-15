using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;
using SunnyBookShop.Utils;
using SunnyBookShop.ViewModels;

namespace SunnyBookShop.Services;

public interface IUserService
{
     Task<User?> AuthenticateAsync(string email, string password);
     Task Login(User user, HttpContext httpContext);
     Task<Result<ClaimsPrincipal>> SignUp(User user);
     Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId);
     Task<ProfileViewModel> GetUserProfileAsync(int userId);
    
}

public class UserService: IUserService
{
    const string authScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    private readonly ApplicationDbContext _dbContext;
    
    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.Email == email && u.Password == password);
    }

    public async Task Login(User user, HttpContext httpContext)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var claimsIdentity = new ClaimsIdentity(claims, authScheme);
        await httpContext.SignInAsync(authScheme, new ClaimsPrincipal(claimsIdentity));
    }

    public async Task<Result<ClaimsPrincipal>> SignUp(User user)
    {
        var errors = new Dictionary<string, string>();

        if (await _dbContext.Users.AnyAsync(u => u.Email == user.Email))
        {
            errors["Email"] = "Email address is already in use.";
        }

        if (user.Password != user.ConfirmPassword)
        {
            errors["ConfirmPassword"] = "Passwords do not match.";
        }

        if (errors.Count > 0)
        {
            return Result<ClaimsPrincipal>.Failure(errors);
        }

        var newUser = new User
        {
            Email = user.Email,
            Password = user.Password,
            Profile = new UserProfile(),
            Role = "User"
        };

        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim("UserId", newUser.Id.ToString()),
            new Claim(ClaimTypes.Email, newUser.Email),
            new Claim(ClaimTypes.Role, newUser.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, authScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return Result<ClaimsPrincipal>.Success(claimsPrincipal);
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
    {
       return await _dbContext.CartItems
           .Include(i => i.User)
           .Include(i => i.Book)
           .Where(i => i.UserId == int.Parse(userId))
           .ToListAsync();
    }

    public async Task<ProfileViewModel> GetUserProfileAsync(int id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        var orders = _dbContext.Orders.Include(o => o.Book)
            .Where(o => o.UserId == id && o.Status != "Delivered").GroupBy(o => o.OrderGroupId);
        var groupedOrders = await orders.Select(g => g.ToList()).ToArrayAsync();
        
        ProfileViewModel profileVM = new()
        {
            User = user,
            Orders = groupedOrders
        };
        return profileVM;
    }
}
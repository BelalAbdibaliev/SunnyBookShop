using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;
using SunnyBookShop.Utils;

namespace SunnyBookShop.Services;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string email, string password);
     Task Login(User user, HttpContext httpContext);
     Task<Result<ClaimsPrincipal>> SignUp(User user);
    
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
}
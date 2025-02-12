using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;
using SunnyBookShop.Utils;

namespace SunnyBookShop.Services;

public interface IUserService
{
     Task<User> GetUserAsync(string username, string password);
     Task<Result<ClaimsPrincipal>> SignUp(User user, string claims);
    
}

public class UserService: IUserService
{
    private readonly ApplicationDbContext _dbContext;
    
    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<User> GetUserAsync(string email, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        
        return user;
    }

    public async Task<Result<ClaimsPrincipal>> SignUp(User user, string authScheme)
    {
        var errors = new Dictionary<string, string>();

        if (_dbContext.Users.Any(u => u.Email == user.Email))
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
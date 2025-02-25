using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Interfaces;
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
     Task<ProfileViewModel> GetUserAsync(int userId);
     Task<UserProfile?> GetUserProfileAsync(int userId);
     Task<CheckoutViewModel> GetCheckoutViewModelAsync(int? userId);
     Task<List<Order>> CheckOut(int userId, decimal totalPrice);
     Task<bool> EditUserProfileAsync(int userId ,UserProfile userProfile);
     Task<List<List<Order>>> GetPurchasesHistoryAsync(int userId);
}

public class UserService: IUserService
{
    const string authScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPhotoService _photoService;
    
    public UserService(ApplicationDbContext dbContext, IPhotoService photoService)
    {
        _dbContext = dbContext;
        _photoService = photoService;
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
        
        var passwordHasher = new PasswordHasher<User>(); 
        string hashedPassword = passwordHasher.HashPassword(null, user.Password);

        var newUser = new User
        {
            Email = user.Email,
            Password = user.Password,
            PasswordHash = hashedPassword,
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

    public async Task<ProfileViewModel> GetUserAsync(int id)
    {
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == id);
        var orders = _dbContext.Orders
            .Include(o => o.Book)
            .Where(o => o.UserId == id && o.Status != "Delivered").GroupBy(o => o.OrderGroupId);
        var groupedOrders = await orders
            .Select(g => g.ToList())
            .ToArrayAsync();
        
        ProfileViewModel profileVM = new()
        {
            User = user,
            Orders = groupedOrders
        };
        return profileVM;
    }

    public async Task<UserProfile?> GetUserProfileAsync(int userId)
    {
        UserProfile? userProfile = await _dbContext.UserProfiles
            .Include(u => u.User)
            .FirstOrDefaultAsync(u => u.UserId == userId);
        return userProfile;
    }

    public async Task<CheckoutViewModel> GetCheckoutViewModelAsync(int? userId)
    {
        var cartItems = await _dbContext.CartItems
            .Include(i => i.User)
            .Include(i => i.Book)
            .Where(i => i.UserId == userId)
            .ToListAsync();

        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        CheckoutViewModel checkoutVM = new()
        {
            CartItems = cartItems,
            User = user
        };
        
        return checkoutVM;
    }

    public async Task<List<Order>> CheckOut(int userId, decimal totalPrice)
    {
        List<Order> orders = new();

        var cartItems = await _dbContext.CartItems
            .Include(i => i.User)
            .Include(i => i.Book)
            .Where(i => i.UserId == userId)
            .ToListAsync();
        string orderGroupId = Guid.NewGuid().ToString();
        foreach (CartItem item in cartItems)
        {
            var order = new Order
            {
                Book = item.Book,
                BookId = item.BookId,
                User = item.User,
                UserId = item.UserId,
                Total = totalPrice,
                OrderGroupId = orderGroupId,
                Status = "Waiting for dispatch"
            };
            item.Book.Count -= 1;
            if (item.Book.Count < 0)
                item.Book.Count = 0;
            orders.Add(order);
            _dbContext.Books.Update(item.Book);
        }

        await _dbContext.Orders.AddRangeAsync(orders);
        _dbContext.CartItems.RemoveRange(cartItems);
        await _dbContext.SaveChangesAsync();

        return orders;
    }

    public async Task<bool> EditUserProfileAsync(int userId ,UserProfile profile)
    {
        var user = await _dbContext.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);
        user.Profile.Name = profile.Name;
        user.Profile.Location = profile.Location;
        user.Profile.PhoneNumber = profile.PhoneNumber;
        
        if (profile.AvatarFile is not null)
        {
            if(profile.AvatarUrl is not null)
                await _photoService.DeletePhotoAsync(user.Profile.AvatarUrl);
            
            var creationResult = await _photoService.UploadPhotoAsync(profile.AvatarFile);
            user.Profile.AvatarUrl = creationResult.Url.ToString();
        }
        
        _dbContext.Users.Update(user);
        if(await _dbContext.SaveChangesAsync() > 0)
            return true;
        
        return false;
    }

    public async Task<List<List<Order>>> GetPurchasesHistoryAsync(int userId)
    {
        var orders = await _dbContext.Orders
            .Include(o => o.Book)
            .Where(o => o.UserId == userId)
            .Where(o => o.Status == "Delivered" || o.Status == "Cancelled")
            .GroupBy(o => o.OrderGroupId)
            .Select(g => g.ToList())
            .ToListAsync();

        return orders;
    }
}
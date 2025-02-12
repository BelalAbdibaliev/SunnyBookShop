using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Models;

namespace SunnyBookShop.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
}
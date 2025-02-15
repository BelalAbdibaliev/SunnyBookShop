using SunnyBookShop.Models;

namespace SunnyBookShop.ViewModels;

public class ProfileViewModel
{
    public User User { get; set; } = null!;
    public IEnumerable<Order>[] Orders { get; set; } = null!;
}
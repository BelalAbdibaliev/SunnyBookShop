using SunnyBookShop.Models;

namespace SunnyBookShop.ViewModels;

public class CheckoutViewModel
{
    public IEnumerable<CartItem> CartItems { get; set; } = null!;
    public User User { get; set; } = new();
    public Order? Order { get; set; } = new();
}
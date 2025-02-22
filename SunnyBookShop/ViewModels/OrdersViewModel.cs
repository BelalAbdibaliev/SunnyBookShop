using SunnyBookShop.Models;

namespace SunnyBookShop.ViewModels;

public class OrdersViewModel
{
    public List<Order>[] Orders { get; set; } = null!;
    public List<Order>? OrdersToUpdate { get; set; }
}
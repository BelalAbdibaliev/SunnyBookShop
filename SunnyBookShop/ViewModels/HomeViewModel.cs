using SunnyBookShop.Models;

namespace SunnyBookShop.ViewModels;

public class HomeViewModel
{
    public IEnumerable<Book> NewBooks { get; set; } = null!;
    public IEnumerable<Book> BestBooks { get; set; } = null!;
    public IEnumerable<Book> CheapBooks { get; set; } = null!;
}
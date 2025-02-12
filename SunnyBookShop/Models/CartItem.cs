namespace SunnyBookShop.Models;

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
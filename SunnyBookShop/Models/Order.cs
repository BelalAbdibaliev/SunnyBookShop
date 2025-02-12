namespace SunnyBookShop.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public string OrderGroupId { get; set; } = null!;
    public string? Status { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
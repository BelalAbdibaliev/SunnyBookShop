using SunnyBookShop.Models;

namespace SunnyBookShop.ViewModels;

public class BookDetailsViewModel
{
    public Book Book { get; set; } = new();
    public User? User { get; set; }
    public CartItem? CartItem { get; set; }
    public IEnumerable<Book> SimilarBooks { get; set; } = null!;
    public Review? Review { get; set; }
    public int? ReviewsCount { get; set; }
    public int? PositiveReviews { get; set; }
    public int? PositiveReviewsPercents { get; set; }
}
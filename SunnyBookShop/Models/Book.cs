using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunnyBookShop.Utils;

namespace SunnyBookShop.Models;

public class Book
{
    public int Id { get; set; }
    [Required] public string Title { get; set; } = null!;
    [Required] public string Author { get; set; } = null!;
    public string? Publisher { get; set; }
    public string? Description { get; set; }
    public string? PosterUrl { get; set; }

    [NotMapped] public IFormFile? PosterFile { get; set; }
    public string? Category { get; set; }

    [Display(Name = "Subcategory")] public string? SubCategory { get; set; }

    [NotMapped]
    public string? CategoryString
    {
        get
        {
            _ = Enum.TryParse(typeof(CategoryList), Category, true, out var category);
            return category?.ToString();
        }
    }

    [Required(ErrorMessage = "Price field is required and must have numeric value.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Count field is required and must have numeric value.")]
    public int Count { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Review> Reviews { get; set; } = new();
}
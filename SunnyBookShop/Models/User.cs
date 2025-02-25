using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyBookShop.Models;

public class User
{
    public int Id { get; set; }

    [DataType(DataType.EmailAddress), Required]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password), MinLength(6), Required]
    public string Password { get; set; } = null!;

    public string? PasswordHash { get; set; } = "NONE";

    [NotMapped]
    [Required(ErrorMessage = "Confirmation Password is required.")]
    [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
    public string? ConfirmPassword { get; set; }

    [NotMapped]
    [Display(Name = "I agree to the terms of the bookshop and the Privacy Policy.")]
    public bool TermsConditions { get; set; }


    public string Role { get; set; } = "User";
    public UserProfile? Profile { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SunnyBookShop.Utils;

namespace SunnyBookShop.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }

    [NotMapped]
    public IFormFile? AvatarFile { get; set; }

    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }

    [NotMapped]
    public string? LocationString
    {
        get
        {
            _ = Enum.TryParse(typeof(LocationList), Location, true, out var location);
            return location?.ToString();
        }
    }

    public int UserId { get; set; }

    [ValidateNever]
    public User User { get; set; } = null!;
}
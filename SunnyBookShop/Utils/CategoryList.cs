using System.ComponentModel.DataAnnotations;

namespace SunnyBookShop.Utils;

public enum CategoryList
{
    Fiction,
    [Display(Name = "Non-Fiction")]
    NonFiction,
    Education
}
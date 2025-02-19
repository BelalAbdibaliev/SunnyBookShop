using Microsoft.AspNetCore.Mvc;
using SunnyBookShop.Services;

namespace SunnyBookShop.Controllers;

public class AdminController: Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
}
using CloudinaryDotNet.Actions;
using SunnyBookShop.Interfaces;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;

namespace SunnyBookShop.Services;

public interface IAdminService
{
    Task<Book> AddBookAsync(Book book);
}


public class AdminService: IAdminService
{
    private readonly IPhotoService _photoService;
    private readonly ApplicationDbContext _dbContext;

    public AdminService(IPhotoService photoService, ApplicationDbContext dbContext)
    {
        _photoService = photoService;
        _dbContext = dbContext;
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        UploadResult uploadResult;
        
        if(book.PosterFile is not null)
        {
            uploadResult = await _photoService.UploadPhotoAsync(book.PosterFile);
            if (uploadResult.Error is not null)
                return null;
            
            book.PosterUrl = uploadResult.Url.ToString();
        }
        await _dbContext.Books.AddAsync(book);
        await _dbContext.SaveChangesAsync();

        return book;
    }
}
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using SunnyBookShop.Interfaces;
using SunnyBookShop.Models;
using SunnyBookShop.Persistence;

namespace SunnyBookShop.Services;

public interface IAdminService
{
    Task<Book> AddBookAsync(Book book);
    Task<Book?> EditBookAsync(int id, Book book);
    Task<bool> DeleteBookAsync(int id);
}


public class AdminService: IAdminService
{
    private readonly IPhotoService _photoService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IPhotoService photoService, ApplicationDbContext dbContext, ILogger<AdminService> logger)
    {
        _photoService = photoService;
        _dbContext = dbContext;
        _logger = logger;
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

    public async Task<Book?> EditBookAsync(int id, Book book)
    {
        if (book.PosterFile is not null)
        {
            var deletionResult = await _photoService.DeletePhotoAsync(book.PosterUrl);
            _logger.LogInformation($"Deletion of {book.PosterUrl}");
            var uploadResult = await _photoService.UploadPhotoAsync(book.PosterFile);
            book.PosterUrl = uploadResult.Url.ToString();
        } 
        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync();
        
        return book;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        if(book is null)
            return false;
        if (book.PosterUrl is not null)
        {
            await _photoService.DeletePhotoAsync(book.PosterUrl);
        }
        
        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
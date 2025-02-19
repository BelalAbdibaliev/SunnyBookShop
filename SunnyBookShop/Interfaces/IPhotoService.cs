using CloudinaryDotNet.Actions;

namespace SunnyBookShop.Interfaces;

public interface IPhotoService
{
    Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string id);
}
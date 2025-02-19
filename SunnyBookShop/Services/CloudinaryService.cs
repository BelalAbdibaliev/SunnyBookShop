using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using SunnyBookShop.Interfaces;
using SunnyBookShop.Utils;

namespace SunnyBookShop.Services;

public class CloudinaryService: IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
    }
    
    public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
            };
            
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        
        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string id)
    {
        var deleteParams = new DeletionParams(id);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        
        return result;
    }
}
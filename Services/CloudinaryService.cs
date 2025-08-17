using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using NeighborhoodServiceFinder.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NeighborhoodServiceFinder.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        // --- NEW METHOD 1: For Profile Pictures ---
        public async Task<ImageUploadResult> UploadProfileImageAsync(IFormFile file)
        {
            // Uses the "Smart Crop" to create a perfect 500x500 square
            var transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("auto");
            return await UploadAsyncInternal(file, transformation);
        }

        // --- NEW METHOD 2: For Gallery Images ---
        public async Task<ImageUploadResult> UploadGalleryImageAsync(IFormFile file)
        {
            // Uses "Fit Inside" to show the whole image, up to 800x800
            var transformation = new Transformation().Height(800).Width(800).Crop("fit");
            return await UploadAsyncInternal(file, transformation);
        }

        // --- Private Helper Method to Avoid Repeating Code ---
        private async Task<ImageUploadResult> UploadAsyncInternal(IFormFile file, Transformation transformation)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = transformation // Use the transformation that was passed in
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }
            return uploadResult;
        }
    }
}
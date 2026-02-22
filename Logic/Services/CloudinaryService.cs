using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Logic.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Logic.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<string?> UploadImageAsync(IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UniqueFilename = true,
                Overwrite = false
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            return result?.SecureUrl?.ToString();
        }

        public async Task<string?> UploadFileAsync(IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UniqueFilename = true,
                Overwrite = false
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            return result?.SecureUrl?.ToString();
        }
    }
}

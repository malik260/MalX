using Microsoft.AspNetCore.Http;

namespace Logic.IServices
{
    public interface ICloudinaryService
    {
        Task<string?> UploadImageAsync(IFormFile file, string folder);
        Task<string?> UploadFileAsync(IFormFile file, string folder);
    }
}

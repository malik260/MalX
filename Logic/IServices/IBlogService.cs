using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IBlogService
    {
        List<BlogVM> GetAllBlogsService();
        Task<HeplerResponseVM> CreateBlogService(BlogDto model, string? coverImageUrl);
        Task<BlogVM> GetBlogByIdMain(string id);
        Task<HeplerResponseVM> GetBlogByIdService(string id);
        Task<HeplerResponseVM> UpdateBlogService(BlogUpdateDto model, string? coverImageUrl);
        Task<HeplerResponseVM> DeleteBlogByIdService(string id);
        Task<HeplerResponseVM> ToggleBlogStatusService(string id);
        Task<HeplerResponseVM> TrackBlogViewAsync(string blogId, string? ipAddress, string? systemName);
    }
}


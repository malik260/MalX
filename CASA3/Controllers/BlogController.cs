using Core.DTOs;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICloudinaryService _cloudinaryService;

        public BlogController(IBlogService blogService, ICloudinaryService cloudinaryService)
        {
            _blogService = blogService;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult Index()
        {
            var blogs = _blogService.GetAllBlogsService();
            return View(blogs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromForm] BlogDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Title))
                    return Json(new { success = false, message = "Blog Title is required." });

                if (string.IsNullOrEmpty(model.Content))
                    return Json(new { success = false, message = "Blog Content is required." });

                string? coverImageUrl = null;
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    if (model.CoverImage.Length > 5 * 1024 * 1024)
                        return Json(new { success = false, message = "Cover image size exceeds 5MB limit." });

                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExtension = Path.GetExtension(model.CoverImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });

                    coverImageUrl = await _cloudinaryService.UploadImageAsync(model.CoverImage, "malx/blog-covers");
                    if (string.IsNullOrEmpty(coverImageUrl))
                        return Json(new { success = false, message = "Failed to upload cover image." });
                }

                var result = await _blogService.CreateBlogService(model, coverImageUrl);
                return Json(result);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while creating the blog. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogById(string id)
        {
            var result = await _blogService.GetBlogByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBlog([FromForm] BlogUpdateDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title))
                    return Json(new { success = false, message = "ID and Blog Title are required." });

                if (string.IsNullOrEmpty(model.Content))
                    return Json(new { success = false, message = "Blog Content is required." });

                string? coverImageUrl = null;
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    if (model.CoverImage.Length > 5 * 1024 * 1024)
                        return Json(new { success = false, message = "Cover image size exceeds 5MB limit." });

                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExtension = Path.GetExtension(model.CoverImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });

                    coverImageUrl = await _cloudinaryService.UploadImageAsync(model.CoverImage, "malx/blog-covers");
                    if (string.IsNullOrEmpty(coverImageUrl))
                        return Json(new { success = false, message = "Failed to upload cover image." });
                }

                var result = await _blogService.UpdateBlogService(model, coverImageUrl);
                return Json(result);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating the blog. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlog(string id)
        {
            var result = await _blogService.DeleteBlogByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBlogStatus(string id)
        {
            var result = await _blogService.ToggleBlogStatusService(id);
            return Json(result);
        }
    }
}

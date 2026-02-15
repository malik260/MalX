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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(IBlogService blogService, IWebHostEnvironment webHostEnvironment)
        {
            _blogService = blogService;
            _webHostEnvironment = webHostEnvironment;
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
                // Validate required fields
                if (string.IsNullOrEmpty(model.Title))
                {
                    return Json(new { success = false, message = "Blog Title is required." });
                }

                if (string.IsNullOrEmpty(model.Content))
                {
                    return Json(new { success = false, message = "Blog Content is required." });
                }

                string coverImageUrl = null;

                // Handle Cover Image upload (optional)
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    // Validate file size (5MB for cover images)
                    if (model.CoverImage.Length > 5 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Cover image size exceeds 5MB limit." });
                    }

                    // Validate image file extension
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExtension = Path.GetExtension(model.CoverImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    {
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var coverImageUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "blog-covers");
                    if (!Directory.Exists(coverImageUploadsFolder))
                    {
                        Directory.CreateDirectory(coverImageUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueCoverImageFileName = $"{Guid.NewGuid()}_{model.CoverImage.FileName}";
                    var coverImageFilePath = Path.Combine(coverImageUploadsFolder, uniqueCoverImageFileName);

                    // Save file
                    using (var stream = new FileStream(coverImageFilePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }

                    coverImageUrl = Path.Combine("uploads", "blog-covers", uniqueCoverImageFileName).Replace("\\", "/");
                }

                // Call service to create blog
                var result = await _blogService.CreateBlogService(model, coverImageUrl);

                return Json(result);
            }
            catch (Exception ex)
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
                // Validate required fields
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title))
                {
                    return Json(new { success = false, message = "ID and Blog Title are required." });
                }

                if (string.IsNullOrEmpty(model.Content))
                {
                    return Json(new { success = false, message = "Blog Content is required." });
                }

                string coverImageUrl = null;

                // Handle Cover Image upload (optional for update)
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    // Validate file size (5MB for cover images)
                    if (model.CoverImage.Length > 5 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Cover image size exceeds 5MB limit." });
                    }

                    // Validate image file extension
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExtension = Path.GetExtension(model.CoverImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    {
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var coverImageUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "blog-covers");
                    if (!Directory.Exists(coverImageUploadsFolder))
                    {
                        Directory.CreateDirectory(coverImageUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueCoverImageFileName = $"{Guid.NewGuid()}_{model.CoverImage.FileName}";
                    var coverImageFilePath = Path.Combine(coverImageUploadsFolder, uniqueCoverImageFileName);

                    // Save file
                    using (var stream = new FileStream(coverImageFilePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }

                    coverImageUrl = Path.Combine("uploads", "blog-covers", uniqueCoverImageFileName).Replace("\\", "/");
                }

                // Call service to update blog
                var result = await _blogService.UpdateBlogService(model, coverImageUrl);

                return Json(result);
            }
            catch (Exception ex)
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


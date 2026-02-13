using Core.DTOs;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    [Authorize]
    public class NewsLetterController : Controller
    {
        private readonly INewsLetterService _newsLetterService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NewsLetterController(INewsLetterService newsLetterService, IWebHostEnvironment webHostEnvironment)
        {
            _newsLetterService = newsLetterService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var newsletters = _newsLetterService.GetAllNewsLetterService();
            return View(newsletters);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewsLetter([FromForm] NewsletterDto model)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Author))
                {
                    return Json(new { success = false, message = "Title and Author are required." });
                }

                // Validate Cover Image
                if (model.CoverImage == null || model.CoverImage.Length == 0)
                {
                    return Json(new { success = false, message = "Cover Image is required." });
                }

                // Validate Document
                if (model.Document == null || model.Document.Length == 0)
                {
                    return Json(new { success = false, message = "Document is required." });
                }

                string coverImageUrl = null;
                string documentUrl = null;

                // Handle Cover Image upload
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    // Validate file size (5MB for images)
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
                    var imageUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "newsletter-images");
                    if (!Directory.Exists(imageUploadsFolder))
                    {
                        Directory.CreateDirectory(imageUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueImageFileName = $"{Guid.NewGuid()}_{model.CoverImage.FileName}";
                    var imageFilePath = Path.Combine(imageUploadsFolder, uniqueImageFileName);

                    // Save file
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }

                    coverImageUrl = Path.Combine("uploads", "newsletter-images", uniqueImageFileName).Replace("\\", "/");
                }

                // Handle Document upload
                if (model.Document != null && model.Document.Length > 0)
                {
                    // Validate file size (10MB for documents)
                    if (model.Document.Length > 10 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Document file size exceeds 10MB limit." });
                    }

                    // Validate document file extension
                    var allowedDocExtensions = new[] { ".pdf", ".doc", ".docx" };
                    var docExtension = Path.GetExtension(model.Document.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedDocExtensions, ext => ext == docExtension))
                    {
                        return Json(new { success = false, message = "Invalid document type. Only PDF, DOC, and DOCX files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var docUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "newsletter-documents");
                    if (!Directory.Exists(docUploadsFolder))
                    {
                        Directory.CreateDirectory(docUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueDocFileName = $"{Guid.NewGuid()}_{model.Document.FileName}";
                    var docFilePath = Path.Combine(docUploadsFolder, uniqueDocFileName);

                    // Save file
                    using (var stream = new FileStream(docFilePath, FileMode.Create))
                    {
                        await model.Document.CopyToAsync(stream);
                    }

                    documentUrl = Path.Combine("uploads", "newsletter-documents", uniqueDocFileName).Replace("\\", "/");
                }

                // Call service to create newsletter
                var result = await _newsLetterService.CreateNewsLetterservice(model, coverImageUrl, documentUrl);

                return Json(result);
            }
            catch (Exception ex)
            {
                // Clean up uploaded files if there was an error
                return Json(new { success = false, message = "An error occurred while creating the newsletter. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsLetterById(string id)
        {
            var result = await _newsLetterService.GetNewsLetterByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNewsLetter([FromForm] NewsletterUpdateDto model)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Author))
                {
                    return Json(new { success = false, message = "ID, Title and Author are required." });
                }

                string coverImageUrl = null;
                string documentUrl = null;

                // Handle Cover Image upload (optional for update)
                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    // Validate file size (5MB for images)
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
                    var imageUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "newsletter-images");
                    if (!Directory.Exists(imageUploadsFolder))
                    {
                        Directory.CreateDirectory(imageUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueImageFileName = $"{Guid.NewGuid()}_{model.CoverImage.FileName}";
                    var imageFilePath = Path.Combine(imageUploadsFolder, uniqueImageFileName);

                    // Save file
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }

                    coverImageUrl = Path.Combine("uploads", "newsletter-images", uniqueImageFileName).Replace("\\", "/");
                }

                // Handle Document upload (optional for update)
                if (model.Document != null && model.Document.Length > 0)
                {
                    // Validate file size (10MB for documents)
                    if (model.Document.Length > 10 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Document file size exceeds 10MB limit." });
                    }

                    // Validate document file extension
                    var allowedDocExtensions = new[] { ".pdf", ".doc", ".docx" };
                    var docExtension = Path.GetExtension(model.Document.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedDocExtensions, ext => ext == docExtension))
                    {
                        return Json(new { success = false, message = "Invalid document type. Only PDF, DOC, and DOCX files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var docUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "newsletter-documents");
                    if (!Directory.Exists(docUploadsFolder))
                    {
                        Directory.CreateDirectory(docUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueDocFileName = $"{Guid.NewGuid()}_{model.Document.FileName}";
                    var docFilePath = Path.Combine(docUploadsFolder, uniqueDocFileName);

                    // Save file
                    using (var stream = new FileStream(docFilePath, FileMode.Create))
                    {
                        await model.Document.CopyToAsync(stream);
                    }

                    documentUrl = Path.Combine("uploads", "newsletter-documents", uniqueDocFileName).Replace("\\", "/");
                }

                // Call service to update newsletter
                var result = await _newsLetterService.UpdateNewsLetterService(model, coverImageUrl, documentUrl);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the newsletter. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNewsLetter(string id)
        {
            var result = await _newsLetterService.DeleteNewsLetterByIdService(id);
            return Json(result);
        }
    }
}

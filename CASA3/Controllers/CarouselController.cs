using Core.DTOs;
using Core.Enum;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    [Authorize]
    public class CarouselController : Controller
    {
        private readonly ICarouselService _carouselService;
        private readonly ICloudinaryService _cloudinaryService;

        public CarouselController(ICarouselService carouselService, ICloudinaryService cloudinaryService)
        {
            _carouselService = carouselService;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult Index()
        {
            var carousels = _carouselService.GetAllCarouselsService();
            return View(carousels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCarousel([FromForm] CarouselDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Title))
                    return Json(new { success = false, message = "Title is required." });

                if (string.IsNullOrEmpty(model.ButtonText))
                    return Json(new { success = false, message = "Button Text is required." });

                if (model.BackgroundImage == null || model.BackgroundImage.Length == 0)
                    return Json(new { success = false, message = "Background Image is required." });

                if (model.PageType == CarouselPageType.Home && (model.Brochure == null || model.Brochure.Length == 0))
                    return Json(new { success = false, message = "Brochure (PDF) is required for Home carousel." });

                if (model.BackgroundImage.Length > 5 * 1024 * 1024)
                    return Json(new { success = false, message = "Background image size exceeds 5MB limit." });

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var imageExtension = Path.GetExtension(model.BackgroundImage.FileName).ToLowerInvariant();
                if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });

                var backgroundImageUrl = await _cloudinaryService.UploadImageAsync(model.BackgroundImage, "malx/carousel-images");
                if (string.IsNullOrEmpty(backgroundImageUrl))
                    return Json(new { success = false, message = "Failed to upload background image." });

                string? brochureUrl = null;
                if (model.PageType == CarouselPageType.Home && model.Brochure != null && model.Brochure.Length > 0)
                {
                    if (model.Brochure.Length > 10 * 1024 * 1024)
                        return Json(new { success = false, message = "Brochure file size exceeds 10MB limit." });

                    if (Path.GetExtension(model.Brochure.FileName).ToLowerInvariant() != ".pdf")
                        return Json(new { success = false, message = "Brochure must be a PDF file." });

                    brochureUrl = await _cloudinaryService.UploadFileAsync(model.Brochure, "malx/carousel-brochures");
                    if (string.IsNullOrEmpty(brochureUrl))
                        return Json(new { success = false, message = "Failed to upload brochure." });
                }

                var result = await _carouselService.CreateCarouselService(model, backgroundImageUrl, brochureUrl);
                return Json(result);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while creating the carousel. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCarouselById(string id)
        {
            var result = await _carouselService.GetCarouselByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCarousel([FromForm] CarouselUpdateDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.ButtonText))
                    return Json(new { success = false, message = "ID, Title and Button Text are required." });

                string? backgroundImageUrl = null;
                if (model.BackgroundImage != null && model.BackgroundImage.Length > 0)
                {
                    if (model.BackgroundImage.Length > 5 * 1024 * 1024)
                        return Json(new { success = false, message = "Background image size exceeds 5MB limit." });

                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExtension = Path.GetExtension(model.BackgroundImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });

                    backgroundImageUrl = await _cloudinaryService.UploadImageAsync(model.BackgroundImage, "malx/carousel-images");
                    if (string.IsNullOrEmpty(backgroundImageUrl))
                        return Json(new { success = false, message = "Failed to upload background image." });
                }

                string? brochureUrl = null;
                if (model.PageType == CarouselPageType.Home && model.Brochure != null && model.Brochure.Length > 0)
                {
                    if (model.Brochure.Length > 10 * 1024 * 1024)
                        return Json(new { success = false, message = "Brochure file size exceeds 10MB limit." });

                    if (Path.GetExtension(model.Brochure.FileName).ToLowerInvariant() != ".pdf")
                        return Json(new { success = false, message = "Brochure must be a PDF file." });

                    brochureUrl = await _cloudinaryService.UploadFileAsync(model.Brochure, "malx/carousel-brochures");
                    if (string.IsNullOrEmpty(brochureUrl))
                        return Json(new { success = false, message = "Failed to upload brochure." });
                }

                var result = await _carouselService.UpdateCarouselService(model, backgroundImageUrl, brochureUrl);
                return Json(result);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating the carousel. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCarousel(string id)
        {
            var result = await _carouselService.DeleteCarouselByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCarouselStatus(string id)
        {
            var result = await _carouselService.ToggleCarouselStatusService(id);
            return Json(result);
        }
    }
}

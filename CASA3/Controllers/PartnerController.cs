using Core.DTOs;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    [Authorize]
    public class PartnerController : Controller
    {
        private readonly IPartnerService _partnerService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PartnerController(IPartnerService partnerService, IWebHostEnvironment webHostEnvironment)
        {
            _partnerService = partnerService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var partners = _partnerService.GetAllPartnersService();
            return View(partners);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePartner([FromForm] PartnerDto model)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(model.Name))
                {
                    return Json(new { success = false, message = "Partner Name is required." });
                }

                // Validate Logo
                if (model.Logo == null || model.Logo.Length == 0)
                {
                    return Json(new { success = false, message = "Partner Logo is required." });
                }

                string logoUrl = null;

                // Handle Logo upload
                if (model.Logo != null && model.Logo.Length > 0)
                {
                    // Validate file size (2MB for logos)
                    if (model.Logo.Length > 2 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Logo size exceeds 2MB limit." });
                    }

                    // Validate image file extension
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
                    var imageExtension = Path.GetExtension(model.Logo.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    {
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, WEBP, and SVG files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var logoUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "partner-logos");
                    if (!Directory.Exists(logoUploadsFolder))
                    {
                        Directory.CreateDirectory(logoUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueLogoFileName = $"{Guid.NewGuid()}_{model.Logo.FileName}";
                    var logoFilePath = Path.Combine(logoUploadsFolder, uniqueLogoFileName);

                    // Save file
                    using (var stream = new FileStream(logoFilePath, FileMode.Create))
                    {
                        await model.Logo.CopyToAsync(stream);
                    }

                    logoUrl = Path.Combine("uploads", "partner-logos", uniqueLogoFileName).Replace("\\", "/");
                }

                // Call service to create partner
                var result = await _partnerService.CreatePartnerService(model, logoUrl);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while creating the partner. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerById(string id)
        {
            var result = await _partnerService.GetPartnerByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePartner([FromForm] PartnerUpdateDto model)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Name))
                {
                    return Json(new { success = false, message = "ID and Partner Name are required." });
                }

                string logoUrl = null;

                // Handle Logo upload (optional for update)
                if (model.Logo != null && model.Logo.Length > 0)
                {
                    // Validate file size (2MB for logos)
                    if (model.Logo.Length > 2 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Logo size exceeds 2MB limit." });
                    }

                    // Validate image file extension
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
                    var imageExtension = Path.GetExtension(model.Logo.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    {
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, WEBP, and SVG files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var logoUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "partner-logos");
                    if (!Directory.Exists(logoUploadsFolder))
                    {
                        Directory.CreateDirectory(logoUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueLogoFileName = $"{Guid.NewGuid()}_{model.Logo.FileName}";
                    var logoFilePath = Path.Combine(logoUploadsFolder, uniqueLogoFileName);

                    // Save file
                    using (var stream = new FileStream(logoFilePath, FileMode.Create))
                    {
                        await model.Logo.CopyToAsync(stream);
                    }

                    logoUrl = Path.Combine("uploads", "partner-logos", uniqueLogoFileName).Replace("\\", "/");
                }

                // Call service to update partner
                var result = await _partnerService.UpdatePartnerService(model, logoUrl);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the partner. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePartner(string id)
        {
            var result = await _partnerService.DeletePartnerByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePartnerStatus(string id)
        {
            var result = await _partnerService.TogglePartnerStatusService(id);
            return Json(result);
        }
    }
}

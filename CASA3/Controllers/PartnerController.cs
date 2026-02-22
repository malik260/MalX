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
        private readonly ICloudinaryService _cloudinaryService;

        public PartnerController(IPartnerService partnerService, ICloudinaryService cloudinaryService)
        {
            _partnerService = partnerService;
            _cloudinaryService = cloudinaryService;
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
                if (string.IsNullOrEmpty(model.Name))
                    return Json(new { success = false, message = "Partner Name is required." });

                if (model.Logo == null || model.Logo.Length == 0)
                    return Json(new { success = false, message = "Partner Logo is required." });

                if (model.Logo.Length > 2 * 1024 * 1024)
                    return Json(new { success = false, message = "Logo size exceeds 2MB limit." });

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
                var imageExtension = Path.GetExtension(model.Logo.FileName).ToLowerInvariant();
                if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, WEBP, and SVG files are allowed." });

                var logoUrl = await _cloudinaryService.UploadImageAsync(model.Logo, "malx/partner-logos");
                if (string.IsNullOrEmpty(logoUrl))
                    return Json(new { success = false, message = "Failed to upload partner logo." });

                var result = await _partnerService.CreatePartnerService(model, logoUrl);
                return Json(result);
            }
            catch (Exception)
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
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Name))
                    return Json(new { success = false, message = "ID and Partner Name are required." });

                string? logoUrl = null;
                if (model.Logo != null && model.Logo.Length > 0)
                {
                    if (model.Logo.Length > 2 * 1024 * 1024)
                        return Json(new { success = false, message = "Logo size exceeds 2MB limit." });

                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
                    var imageExtension = Path.GetExtension(model.Logo.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, WEBP, and SVG files are allowed." });

                    logoUrl = await _cloudinaryService.UploadImageAsync(model.Logo, "malx/partner-logos");
                    if (string.IsNullOrEmpty(logoUrl))
                        return Json(new { success = false, message = "Failed to upload partner logo." });
                }

                var result = await _partnerService.UpdatePartnerService(model, logoUrl);
                return Json(result);
            }
            catch (Exception)
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

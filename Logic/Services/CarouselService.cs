using Core.DB;
using Core.DTOs;
using Core.Enum;
using Core.Models;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class CarouselService : ICarouselService
    {
        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public CarouselService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<CarouselVM> GetAllCarouselsService()
        {
            try
            {
                var carousels = _context.Carousels
                    .Where(a => !a.IsDeleted)
                    .Include(u => u.CreatedBy)
                    .OrderBy(c => c.PageType)
                    .ThenBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Title)
                    .ToList()
                    .Select(MapCarouselToVM)
                    .ToList();

                if (carousels.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No Carousels found.");
                    return new List<CarouselVM>();
                }

                return carousels;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<CarouselVM>();
            }
        }

        public List<CarouselVM> GetCarouselsByPageTypeService(CarouselPageType pageType)
        {
            try
            {
                var carousels = _context.Carousels
                    .Where(a => !a.IsDeleted && a.PageType == pageType && a.IsActive)
                    .Include(u => u.CreatedBy)
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.Title)
                    .ToList()
                    .Select(MapCarouselToVM)
                    .ToList();

                return carousels;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<CarouselVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateCarouselService(CarouselDto model, string backgroundImageUrl, string? brochureUrl)
        {
            var response = new HeplerResponseVM();
            try
            {
                var cuser = Util.GetCurrentUser();
                if (cuser == null)
                {
                    response.Message = "Unauthorized User";
                    return response;
                }

                if (model != null && !string.IsNullOrEmpty(backgroundImageUrl))
                {
                    if (!string.IsNullOrEmpty(model.Title) && !string.IsNullOrEmpty(model.ButtonText))
                    {
                        // Validate based on page type
                        if (model.PageType == CarouselPageType.Home && string.IsNullOrEmpty(brochureUrl))
                        {
                            response.Message = "Brochure is required for Home carousel";
                            return response;
                        }

                        var carousel = new Carousel()
                        {
                            Title = model.Title,
                            Subtitle = model.Subtitle,
                            PageType = model.PageType,
                            BackgroundImageUrl = backgroundImageUrl,
                            BadgeText = model.BadgeText,
                            BrochureUrl = brochureUrl,
                            ButtonLink = model.ButtonLink,
                            ButtonText = model.ButtonText,
                            DisplayOrder = 0,
                            IsActive = true,
                            CreatedById = "6e4a48e0-0dc7-4e6d-b4a3-cce4d2bf9fbd"
                        };

                        await _context.AddAsync(carousel).ConfigureAwait(false);
                        await _context.SaveChangesAsync();

                        response.success = true;
                        response.Message = "Carousel Created Successfully";
                        response.Data = MapCarouselToVM(carousel);
                        return response;
                    }
                }

                response.Message = "Invalid Parameter Submitted";
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<CarouselVM> GetCarouselByIdMain(string id)
        {
            try
            {
                var model = await _context.Carousels
                    .Include(x => x.CreatedBy)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (model != null)
                {
                    return MapCarouselToVM(model);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> GetCarouselByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var record = await GetCarouselByIdMain(id);
                if (record != null)
                {
                    response.success = true;
                    response.Message = "Successful";
                    response.Data = record;
                }
                else
                {
                    response.success = false;
                    response.Message = "No Record Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception logged";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> UpdateCarouselService(CarouselUpdateDto model, string? backgroundImageUrl, string? brochureUrl)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.ButtonText))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var existingRecord = await _context.Carousels
                    .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

                if (existingRecord == null)
                {
                    response.Message = "No Record Found";
                    return response;
                }

                // Update fields
                existingRecord.Title = model.Title;
                existingRecord.Subtitle = model.Subtitle;
                existingRecord.PageType = model.PageType;
                existingRecord.BadgeText = model.BadgeText;
                existingRecord.ButtonLink = model.ButtonLink;
                existingRecord.ButtonText = model.ButtonText;
                existingRecord.UpdatedAt = DateTime.UtcNow;

                // Update display order if provided
                if (model.DisplayOrder.HasValue)
                {
                    existingRecord.DisplayOrder = model.DisplayOrder.Value;
                }

                // Update active status if provided
                if (model.IsActive.HasValue)
                {
                    existingRecord.IsActive = model.IsActive.Value;
                }

                // If new background image is provided, delete old image and update URL
                if (!string.IsNullOrEmpty(backgroundImageUrl))
                {
                    DeleteFileIfExists(existingRecord.BackgroundImageUrl);
                    existingRecord.BackgroundImageUrl = backgroundImageUrl;
                }

                // If new brochure is provided, delete old brochure and update URL
                if (!string.IsNullOrEmpty(brochureUrl))
                {
                    DeleteFileIfExists(existingRecord.BrochureUrl);
                    existingRecord.BrochureUrl = brochureUrl;
                }

                _context.Update(existingRecord);
                await _context.SaveChangesAsync();

                response.success = true;
                response.Message = "Updated Successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> DeleteCarouselByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                // Get the record first to delete associated files
                var carousel = await _context.Carousels
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (carousel != null)
                {
                    // Delete background image and brochure files from server
                    DeleteFileIfExists(carousel.BackgroundImageUrl);
                    DeleteFileIfExists(carousel.BrochureUrl);
                }

                // Soft delete the record
                var rex = await _context.Carousels
                    .Where(v => v.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(v => v.IsDeleted, true)
                        .SetProperty(v => v.UpdatedAt, DateTime.UtcNow));

                if (rex > 0)
                {
                    response.success = true;
                    response.Message = "Deleted Successfully";
                }
                else
                {
                    response.success = false;
                    response.Message = "No Record Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> ToggleCarouselStatusService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var carousel = await _context.Carousels
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (carousel != null)
                {
                    carousel.IsActive = !carousel.IsActive;
                    carousel.UpdatedAt = DateTime.UtcNow;

                    _context.Update(carousel);
                    await _context.SaveChangesAsync();

                    response.success = true;
                    response.Message = $"Carousel {(carousel.IsActive ? "Activated" : "Deactivated")} Successfully";
                    response.Data = carousel.IsActive;
                }
                else
                {
                    response.success = false;
                    response.Message = "No Record Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public CarouselVM MapCarouselToVM(Carousel carousel)
        {
            if (carousel == null) return null;

            return new CarouselVM
            {
                Id = carousel.Id,
                Title = carousel.Title,
                Subtitle = carousel.Subtitle,
                PageType = carousel.PageType,
                PageTypeDisplay = carousel.PageType.ToString(),
                BackgroundImageUrl = carousel.BackgroundImageUrl,
                BadgeText = carousel.BadgeText,
                BrochureUrl = carousel.BrochureUrl,
                ButtonLink = carousel.ButtonLink,
                ButtonText = carousel.ButtonText,
                DisplayOrder = carousel.DisplayOrder,
                IsActive = carousel.IsActive,
                CreatedAt = carousel.CreatedAt,
                UpdatedAt = carousel.UpdatedAt,
                CreatedBy = carousel.CreatedBy != null
                    ? $"{carousel.CreatedBy.FirstName} {carousel.CreatedBy.LastName}"
                    : "N/A",
            };
        }

        // Helper method to delete files from server
        private void DeleteFileIfExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, $"Deleted file: {filePath}");
                }
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"Failed to delete file {filePath}: {ex.Message}");
            }
        }
    }
}

using Core.DB;
using Core.DTOs;
using Core.Models;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public PartnerService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<PartnerVM> GetAllPartnersService()
        {
            try
            {
                var partners = _context.Partners
                    .Where(a => !a.IsDeleted)
                    .Include(u => u.CreatedBy)
                    .OrderBy(p => p.DisplayOrder)
                    .ThenBy(p => p.Name)
                    .ToList()
                    .Select(MapPartnerToVM)
                    .ToList();

                if (partners.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No Partners found.");
                    return new List<PartnerVM>();
                }

                return partners;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<PartnerVM>();
            }
        }

        public async Task<HeplerResponseVM> CreatePartnerService(PartnerDto model, string logoUrl)
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

                if (model != null && !string.IsNullOrEmpty(logoUrl))
                {
                    if (!string.IsNullOrEmpty(model.Name))
                    {
                        var partner = new Partner()
                        {
                            Name = model.Name,
                            Address = model.Address,
                            ContactEmail = model.ContactEmail,
                            ContactPhone = model.ContactPhone,
                            LogoUrl = logoUrl,
                            DisplayOrder = 0,
                            IsActive = true,
                            CreatedById = cuser.Id
                        };

                        await _context.AddAsync(partner).ConfigureAwait(false);
                        await _context.SaveChangesAsync();

                        response.success = true;
                        response.Message = "Partner Created Successfully";
                        response.Data = MapPartnerToVM(partner);
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

        public async Task<PartnerVM> GetPartnerByIdMain(string id)
        {
            try
            {
                var model = await _context.Partners.Include(x => x.CreatedBy).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (model != null)
                {
                    return MapPartnerToVM(model);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> GetPartnerByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var record = await GetPartnerByIdMain(id);
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

        public async Task<HeplerResponseVM> UpdatePartnerService(PartnerUpdateDto model, string? logoUrl)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Name))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var existingRecord = await _context.Partners
                    .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

                if (existingRecord == null)
                {
                    response.Message = "No Record Found";
                    return response;
                }

                // Update fields
                existingRecord.Name = model.Name;
                existingRecord.Address = model.Address;
                existingRecord.ContactEmail = model.ContactEmail;
                existingRecord.ContactPhone = model.ContactPhone;
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

                // If new logo is provided, delete old logo and update URL
                if (!string.IsNullOrEmpty(logoUrl))
                {
                    DeleteFileIfExists(existingRecord.LogoUrl);
                    existingRecord.LogoUrl = logoUrl;
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

        public async Task<HeplerResponseVM> DeletePartnerByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                // Get the record first to delete associated logo file
                var partner = await _context.Partners
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (partner != null)
                {
                    // Delete logo file from server
                    DeleteFileIfExists(partner.LogoUrl);
                }

                // Soft delete the record
                var rex = await _context.Partners
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

        public async Task<HeplerResponseVM> TogglePartnerStatusService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var partner = await _context.Partners
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (partner != null)
                {
                    partner.IsActive = !partner.IsActive;
                    partner.UpdatedAt = DateTime.UtcNow;

                    _context.Update(partner);
                    await _context.SaveChangesAsync();

                    response.success = true;
                    response.Message = $"Partner {(partner.IsActive ? "Activated" : "Deactivated")} Successfully";
                    response.Data = partner.IsActive;
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

        public PartnerVM MapPartnerToVM(Partner partner)
        {
            if (partner == null) return null;

            return new PartnerVM
            {
                Id = partner.Id,
                Name = partner.Name,
                Address = partner.Address,
                ContactEmail = partner.ContactEmail,
                ContactPhone = partner.ContactPhone,
                LogoUrl = partner.LogoUrl,
                DisplayOrder = partner.DisplayOrder,
                IsActive = partner.IsActive,
                CreatedAt = partner.CreatedAt,
                UpdatedAt = partner.UpdatedAt,
                CreatedBy = partner.CreatedBy != null
                    ? $"{partner.CreatedBy.FirstName} {partner.CreatedBy.LastName}"
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

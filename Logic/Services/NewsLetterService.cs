using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.Models;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class NewsLetterService : INewsLetterService
    {
        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public NewsLetterService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<NewsLetterVM> GetAllNewsLetterService()
        {
            try
            {
                var NewsLetters = _context.NewsLetters.Where(a => !a.IsDeleted).Include(u => u.CreatedBy)
                    .ToList()
                    .Select(MapNewsLetterToVM)
                    .ToList();

                if (NewsLetters.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No NewsLetter found.");
                    return new List<NewsLetterVM>();
                }

                return NewsLetters;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<NewsLetterVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateNewsLetterservice(NewsletterDto model, string coverImgUrl, string documentUrl)
        {
            var response = new HeplerResponseVM();
            try
            {
                var cuser = Util.GetCurrentUser();
                if (cuser == null)
                {
                    response.Message = "Unauthorized User"; return response;
                }
                if (model != null && !string.IsNullOrEmpty(coverImgUrl) && !string.IsNullOrEmpty(documentUrl))
                {
                    if (!string.IsNullOrEmpty(model.Title) && !string.IsNullOrEmpty(model.Author))
                    {
                        var nl = new NewsLetter()
                        {
                            Title = model?.Title!,
                            Description = model?.Description!,
                            Author = model.Author,
                            CoverImageUrl = coverImgUrl,
                            DocumentUrl = documentUrl,
                            CreatedById = cuser.Id
                        };
                        await _context.AddAsync(nl).ConfigureAwait(false);
                        await _context.SaveChangesAsync();
                        response.success = true;
                        response.Message = "Created Successfully";
                        response.Data = MapNewsLetterToVM(nl);
                        return response;
                    }
                }
                response.Message = "Invalid Parameter Submitted"; return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<NewsLetterVM> GetNewsLetterIdMain(string id)
        {
            try
            {
                var model = await _context.NewsLetters.Include(x => x.CreatedBy).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (model != null)
                {
                    return MapNewsLetterToVM(model);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> GetNewsLetterByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var record = await GetNewsLetterIdMain(id);
                if (record != null)
                {
                    response.success = true; response.Message = "Successful"; response.Data = record;
                }
                else
                {
                    response.success = false; response.Message = "No Record Found";
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

        public async Task<HeplerResponseVM> UpdateNewsLetterService(NewsletterUpdateDto model, string? coverImgUrl, string? documentUrl)
        {
            var response = new HeplerResponseVM();
            try
            {
                // Fixed validation logic - should use OR (||) not AND (&&)
                if (model == null || string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Author) || string.IsNullOrEmpty(model.Title))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }

                var existingRecord = await _context.NewsLetters.FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);
                if (existingRecord == null)
                {
                    response.Message = "No Record Found"; return response;
                }

                // Update fields
                existingRecord.Title = model.Title;
                existingRecord.Author = model.Author;
                existingRecord.Description = model.Description;
                existingRecord.UpdatedAt = DateTime.UtcNow;

                // If new files are provided, delete old files and update URLs
                if (!string.IsNullOrEmpty(coverImgUrl))
                {
                    // Delete old cover image file
                    DeleteFileIfExists(existingRecord.CoverImageUrl);
                    existingRecord.CoverImageUrl = coverImgUrl;
                }

                if (!string.IsNullOrEmpty(documentUrl))
                {
                    // Delete old document file
                    DeleteFileIfExists(existingRecord.DocumentUrl);
                    existingRecord.DocumentUrl = documentUrl;
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

        public async Task<HeplerResponseVM> DeleteNewsLetterByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }

                // Get the record first to delete associated files
                var newsletter = await _context.NewsLetters.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (newsletter != null)
                {
                    // Delete associated files from server
                    DeleteFileIfExists(newsletter.CoverImageUrl);
                    DeleteFileIfExists(newsletter.DocumentUrl);
                }

                // Soft delete the record
                var rex = await _context.NewsLetters.Where(v => v.Id == id).ExecuteUpdateAsync(setters => setters
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

        public NewsLetterVM MapNewsLetterToVM(NewsLetter NewsLetter)
        {
            if (NewsLetter == null) return null;
            return new NewsLetterVM
            {
                Id = NewsLetter.Id,
                Title = NewsLetter.Title,
                Description = NewsLetter.Description,
                Author = NewsLetter.Author,
                CoverImageUrl = NewsLetter.CoverImageUrl,
                DocumentUrl = NewsLetter.DocumentUrl,
                CreatedAt = NewsLetter.CreatedAt,
                UpdatedAt = NewsLetter.UpdatedAt,
                CreatedBy = NewsLetter.CreatedBy != null ? $"{NewsLetter.CreatedBy.FirstName} {NewsLetter.CreatedBy.LastName}" : "N/A",
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
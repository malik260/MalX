using Core.DB;
using Core.DTOs;
using Core.Models;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class BlogService : IBlogService
    {
        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public BlogService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<BlogVM> GetAllBlogsService()
        {
            try
            {
                var blogs = _context.Blogs
                    .Where(a => !a.IsDeleted)
                    .Include(u => u.CreatedBy)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList()
                    .Select(MapBlogToVM)
                    .ToList();

                if (blogs.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No Blogs found.");
                    return new List<BlogVM>();
                }

                return blogs;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<BlogVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateBlogService(BlogDto model, string? coverImageUrl)
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

                if (model != null && !string.IsNullOrEmpty(model.Title) && !string.IsNullOrEmpty(model.Content))
                {
                    // Generate slug if not provided
                    var slug = model.Slug;
                    if (string.IsNullOrWhiteSpace(slug))
                    {
                        slug = GenerateSlug(model.Title);
                    }

                    // Check if slug already exists
                    var existingSlug = await _context.Blogs
                        .FirstOrDefaultAsync(b => b.Slug == slug && !b.IsDeleted);
                    
                    if (existingSlug != null)
                    {
                        slug = $"{slug}-{DateTime.UtcNow.Ticks}";
                    }

                    var blog = new Blog()
                    {
                        Title = model.Title.Trim(),
                        Slug = slug,
                        Category = string.IsNullOrWhiteSpace(model.Category) ? null : model.Category.Trim(),
                        PublishedDate = model.PublishedDate ?? DateTime.UtcNow,
                        Content = model.Content,
                        Excerpt = string.IsNullOrWhiteSpace(model.Excerpt) ? null : model.Excerpt.Trim(),
                        CoverImageUrl = coverImageUrl,
                        Author = string.IsNullOrWhiteSpace(model.Author) ? null : model.Author.Trim(),
                        Views = 0,
                        IsPublished = model.IsPublished,
                        CreatedById = cuser.Id
                    };

                    await _context.AddAsync(blog).ConfigureAwait(false);
                    await _context.SaveChangesAsync();

                    response.success = true;
                    response.Message = "Blog Created Successfully";
                    response.Data = MapBlogToVM(blog);
                    return response;
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

        public async Task<BlogVM> GetBlogByIdMain(string id)
        {
            try
            {
                var model = await _context.Blogs.Include(x => x.CreatedBy).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (model != null)
                {
                    return MapBlogToVM(model);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> GetBlogByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var record = await GetBlogByIdMain(id);
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

        public async Task<HeplerResponseVM> UpdateBlogService(BlogUpdateDto model, string? coverImageUrl)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Content))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var existingRecord = await _context.Blogs
                    .FirstOrDefaultAsync(x => x.Id == model.Id && !x.IsDeleted);

                if (existingRecord == null)
                {
                    response.Message = "No Record Found";
                    return response;
                }

                // Generate slug if not provided or if title changed
                var slug = model.Slug;
                if (string.IsNullOrWhiteSpace(slug) || existingRecord.Title != model.Title)
                {
                    slug = GenerateSlug(model.Title);
                    
                    // Check if slug already exists (excluding current record)
                    var existingSlug = await _context.Blogs
                        .FirstOrDefaultAsync(b => b.Slug == slug && b.Id != model.Id && !b.IsDeleted);
                    
                    if (existingSlug != null)
                    {
                        slug = $"{slug}-{DateTime.UtcNow.Ticks}";
                    }
                }

                // Update fields
                existingRecord.Title = model.Title.Trim();
                existingRecord.Slug = slug;
                existingRecord.Category = string.IsNullOrWhiteSpace(model.Category) ? null : model.Category.Trim();
                existingRecord.PublishedDate = model.PublishedDate ?? existingRecord.PublishedDate;
                existingRecord.Content = model.Content;
                existingRecord.Excerpt = string.IsNullOrWhiteSpace(model.Excerpt) ? null : model.Excerpt.Trim();
                existingRecord.Author = string.IsNullOrWhiteSpace(model.Author) ? null : model.Author.Trim();
                existingRecord.IsPublished = model.IsPublished;
                existingRecord.UpdatedAt = DateTime.UtcNow;

                // If new cover image is provided, delete old image and update URL
                if (!string.IsNullOrEmpty(coverImageUrl))
                {
                    DeleteFileIfExists(existingRecord.CoverImageUrl);
                    existingRecord.CoverImageUrl = coverImageUrl;
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

        public async Task<HeplerResponseVM> DeleteBlogByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                // Get the record first to delete associated cover image file
                var blog = await _context.Blogs
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (blog != null)
                {
                    // Delete cover image file from server
                    DeleteFileIfExists(blog.CoverImageUrl);
                }

                // Soft delete the record
                var rex = await _context.Blogs
                    .Where(b => b.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.IsDeleted, true)
                        .SetProperty(b => b.UpdatedAt, DateTime.UtcNow));

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

        public async Task<HeplerResponseVM> ToggleBlogStatusService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var blog = await _context.Blogs
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

                if (blog != null)
                {
                    blog.IsPublished = !blog.IsPublished;
                    blog.UpdatedAt = DateTime.UtcNow;

                    _context.Update(blog);
                    await _context.SaveChangesAsync();

                    response.success = true;
                    response.Message = $"Blog {(blog.IsPublished ? "Published" : "Unpublished")} Successfully";
                    response.Data = blog.IsPublished;
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

        public BlogVM MapBlogToVM(Blog blog)
        {
            if (blog == null) return null;

            return new BlogVM
            {
                Id = blog.Id,
                Title = blog.Title,
                Slug = blog.Slug,
                Category = blog.Category,
                PublishedDate = blog.PublishedDate,
                Content = blog.Content,
                Excerpt = blog.Excerpt,
                CoverImageUrl = blog.CoverImageUrl,
                Author = blog.Author,
                Views = blog.Views,
                IsPublished = blog.IsPublished,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt,
                CreatedBy = blog.CreatedBy != null
                    ? $"{blog.CreatedBy.FirstName} {blog.CreatedBy.LastName}"
                    : "N/A",
            };
        }

        // Helper method to generate URL-friendly slug
        private string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            var slug = title.ToLowerInvariant()
                .Trim()
                .Replace(" ", "-")
                .Replace("--", "-");

            // Remove special characters except hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple consecutive hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

            // Remove leading/trailing hyphens
            slug = slug.Trim('-');

            return slug;
        }

        public async Task<HeplerResponseVM> TrackBlogViewAsync(string blogId, string? ipAddress, string? systemName)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(blogId))
                {
                    response.Message = "Invalid Blog ID";
                    return response;
                }

                var blog = await _context.Blogs
                    .FirstOrDefaultAsync(b => b.Id == blogId && !b.IsDeleted);

                if (blog == null)
                {
                    response.Message = "Blog not found";
                    return response;
                }

                // Check if this IP has already viewed this blog
                var existingView = await _context.BlogViews
                    .FirstOrDefaultAsync(bv => bv.BlogId == blogId && bv.IpAddress == ipAddress);

                if (existingView != null)
                {
                    response.success = true;
                    response.Message = "View already tracked for this IP";
                    return response;
                }

                // Create blog view record (first time view from this IP)
                var blogView = new BlogView
                {
                    BlogId = blogId,
                    IpAddress = ipAddress,
                    SystemName = systemName,
                    ViewedAt = DateTime.UtcNow
                };

                await _context.BlogViews.AddAsync(blogView);

                // Increment view count only for new IP
                blog.Views++;
                blog.UpdatedAt = DateTime.UtcNow;
                _context.Update(blog);

                await _context.SaveChangesAsync();

                response.success = true;
                response.Message = "View tracked successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed to track view";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        // Helper method to delete files from server
        private void DeleteFileIfExists(string? filePath)
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


using CASA3.Models;
using Core.DTOs;
using Core.Enum;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CASA3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVendorService _vendorService;
        private readonly IAffiliateService _affiliateService;
        private readonly INewsletterSubscriptionService _newsletterSubscriptionService;
        private readonly IContactUsService _contactUsService;
        private readonly IStaffService _staffService;
        private readonly IProjectService _projectService;
        private readonly INewsLetterService _newsLetter;
        private readonly IPartnerService _partnerService;
        private readonly ICarouselService _carouselService;
        private readonly IBlogService _blogService;

        public HomeController(ILogger<HomeController> logger, IVendorService vendorService, IAffiliateService affiliateService, INewsletterSubscriptionService newsletterSubscriptionService, IContactUsService contactUsService, IStaffService staffService, IProjectService projectService, INewsLetterService newsLetter, IPartnerService partnerService, ICarouselService carouselService, IBlogService blogService)
        {
            _logger = logger;
            _vendorService = vendorService;
            _affiliateService = affiliateService;
            _newsletterSubscriptionService = newsletterSubscriptionService;
            _contactUsService = contactUsService;
            _staffService = staffService;
            _projectService = projectService;
            _newsLetter = newsLetter;
            _partnerService = partnerService;
            _carouselService = carouselService;
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            var model = new HomePageVM();
            model.Banner = _carouselService.GetAllCarouselsService().Where(x => x.PageType == CarouselPageType.Home && x.IsActive).ToList();

            // Featured Projects data
            model.FeaturedProjects = _projectService.GetAllProjects().Where(p=>p.IsFeatured).ToList();

            // CASA III Footprint data - Fetch from database and group by year
            var allProjects = _projectService.GetAllProjects();
            var projectsWithYear = allProjects.Where(p => p.Year.HasValue).ToList();
            
            model.FootprintYears = projectsWithYear
                .GroupBy(p => p.Year!.Value)
                .OrderByDescending(g => g.Key)
                .Select(g => new FootprintYearDto
                {
                    Year = g.Key,
                    Projects = g.Select(p => new FootprintProjectDto
                    {
                        Id = p.Id,
                        Title = p.Name,
                        Subtitle = p.Slug ?? string.Empty,
                        ImageUrl = p.HeroImageUrl,
                        Url = $"/projects/{p.Id}" 
                    }).ToList()
                })
                .ToList();
            
            // Set selected year to the most recent year, or default to 2018 if no projects
            model.SelectedFootprintYear = model.FootprintYears.Any() 
                ? model.FootprintYears.First().Year 
                : DateTime.Now.Year;

            // Newsletters data
            model.Newsletters = _newsLetter.GetAllNewsLetterService();

            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();
            
            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            return View(model);
        }

        private List<ProjectDto> GetProjects()
        {
            var fromDb = _projectService.GetAllProjects();
            if (fromDb != null && fromDb.Any())
                return fromDb;
            return new List<ProjectDto>();
        }

        private List<PartnerVM> GetPartners()
        {
            return _partnerService.GetAllPartnersService();
        }

        public IActionResult AboutUs()
        {
            var model = new HomePageVM();

            var teamMembers = _staffService.GetAllStaffAsTeamMembers();
            // Board of Directors
            model.BoardOfDirectors = teamMembers.Where(tm => tm.Category == TeamMemeberCategory.BOD).ToList();

            // Management Team data
            model.ManagementTeam = teamMembers.Where(tm => tm.Category == TeamMemeberCategory.MGT).ToList();

            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();
            
            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();
            
            ViewData["Title"] = "About Us";
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamMember(string id)
        {
            var member = await _staffService.GetStaffByIdAsync(id);
            if (member == null)
                member = GetAllTeamMembers().FirstOrDefault(x => x.Id == id);
            if (member == null)
                return NotFound();
            return Json(member);
        }


        public IActionResult ContactUs()
        {
            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();

            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            ViewData["Title"] = "About Us";

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactFormDto form)
        {
            // Basic server-side validation
            if (form == null || string.IsNullOrWhiteSpace(form.FirstName) || string.IsNullOrWhiteSpace(form.Email) || string.IsNullOrWhiteSpace(form.Message))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Please fill required fields." });
                }

                TempData["ContactError"] = "Please fill required fields.";
                return RedirectToAction("ContactUs");
            }

            var res = await _contactUsService.CreateContactUsService(form);
            return Json(res);
        }


        [HttpPost]
        public async Task<IActionResult> SubscribeNewsletter(NewsletterSubscriptionDto subscription)
        {
            // Basic validation
            if (subscription == null || string.IsNullOrWhiteSpace(subscription.Email))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Please enter a valid email address." });
                }

                TempData["NewsletterError"] = "Please enter a valid email address.";
                return RedirectToAction("Index");
            }

            // Validate email format
            var emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(subscription.Email, emailRegex))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Please enter a valid email address." });
                }

                TempData["NewsletterError"] = "Please enter a valid email address.";
                return RedirectToAction("Index");
            }

            var res = await _newsletterSubscriptionService.CreateNewsletterSubscriptionsService(subscription);
            return Json(res);
        }

        public IActionResult Blog(int page = 1, string search = "")
        {
            var model = new HomePageVM();

            // Fetch published blogs from database
            var allBlogs = _blogService.GetAllBlogsService()
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.PublishedDate ?? b.CreatedAt)
                .ToList();

            // Convert BlogVM to BlogPostDto
            var allBlogPosts = allBlogs.Select((blog, index) => new BlogPostDto
            {
                Id = index + 1, // Sequential ID for display purposes (view uses this for display)
                Title = blog.Title,
                Slug = blog.Slug ?? blog.Id, // Use slug if available, otherwise use blog ID for navigation
                Category = blog.Category ?? string.Empty,
                PublishedDate = blog.PublishedDate ?? blog.CreatedAt,
                Content = blog.Content,
                Excerpt = blog.Excerpt ?? string.Empty,
                CoverImageUrl = blog.CoverImageUrl ?? string.Empty,
                Author = blog.Author ?? string.Empty,
                Views = blog.Views
            }).ToList();

            // Store blog IDs mapping (sequential display ID -> actual blog ID) for BlogDetails
            var blogIdMapping = allBlogPosts
                .Select((post, idx) => new { DisplayId = post.Id, BlogId = allBlogs[idx].Id })
                .ToDictionary(x => x.DisplayId, x => x.BlogId);
            ViewData["BlogIdMapping"] = blogIdMapping;

            // Filter by search query
            var filteredPosts = allBlogPosts;
            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredPosts = allBlogPosts
                    .Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || (b.Category != null && b.Category.Contains(search, StringComparison.OrdinalIgnoreCase))
                             || (b.Excerpt != null && b.Excerpt.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Setup pagination
            int pageSize = 5;
            int totalItems = filteredPosts.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Validate page number
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            model.Pagination = new PaginationVM
            {
                TotalItems = totalItems,
                ItemsPerPage = pageSize,
                CurrentPage = page,
                SearchQuery = search ?? ""
            };

            // Apply pagination using Skip and Take
            model.BlogPosts = filteredPosts.OrderByDescending(b => b.PublishedDate).Skip(model.Pagination.SkipCount).Take(pageSize).ToList();

            // Recent posts - Get the 3 most recent from all posts (not filtered)
            model.RecentBlogPosts = allBlogPosts.OrderByDescending(b => b.PublishedDate).Take(3).ToList();

            // Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    blogPosts = model.BlogPosts,
                    pagination = new
                    {
                        currentPage = model.Pagination.CurrentPage,
                        totalPages = model.Pagination.TotalPages,
                        totalItems = model.Pagination.TotalItems,
                        searchQuery = model.Pagination.SearchQuery,
                        hasPreviousPage = model.Pagination.HasPreviousPage,
                        hasNextPage = model.Pagination.HasNextPage
                    }
                });
            }

            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();

            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            ViewData["Title"] = "Blog";

            return View(model);
        }

        public IActionResult OurProject()
        {
            // Get all projects from database
            var model = _projectService.GetAllProjects();

            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();
            
            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult BecomeAnAffiliate()
        {
            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();

            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            return View("BecomeAnAffiliate");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAffiliate(AffiliateRegistrationDto registration)
        {
            // Basic validation
            if (registration == null || string.IsNullOrWhiteSpace(registration.FirstName) || string.IsNullOrWhiteSpace(registration.LastName))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "First Name and Last Name are required." });
                }
                TempData["AffiliateError"] = "First Name and Last Name are required.";
                return RedirectToAction("Affiliate");
            }

            // Validate email format
            var emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            if (string.IsNullOrWhiteSpace(registration.Email) || !System.Text.RegularExpressions.Regex.IsMatch(registration.Email, emailRegex))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Please enter a valid email address." });
                }
                TempData["AffiliateError"] = "Please enter a valid email address.";
                return RedirectToAction("Affiliate");
            }

            var res = await _affiliateService.CreateAffiliateService(registration);
            return Json(res);
        }

        public async Task<IActionResult> BlogDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Try to get blog by ID first
            var blogResult = await _blogService.GetBlogByIdService(id);
            
            // If not found by ID, try to find by slug
            if (blogResult == null || !blogResult.success)
            {
                var allBlogs = _blogService.GetAllBlogsService()
                    .Where(b => b.IsPublished && !string.IsNullOrEmpty(b.Slug) && b.Slug == id)
                    .ToList();
                
                var blogBySlug = allBlogs.FirstOrDefault();
                if (blogBySlug != null)
                {
                    blogResult = await _blogService.GetBlogByIdService(blogBySlug.Id);
                }
            }

            if (blogResult == null || !blogResult.success)
                return NotFound();

            var blog = blogResult.Data as Core.ViewModels.BlogVM;
            if (blog == null)
                return NotFound();

            // Convert BlogVM to BlogPostDto for the view
            var post = new BlogPostDto
            {
                Id = 0, // Not used in details view
                Title = blog.Title,
                Slug = blog.Slug ?? string.Empty,
                Category = blog.Category ?? string.Empty,
                PublishedDate = blog.PublishedDate ?? DateTime.UtcNow,
                Content = blog.Content,
                Excerpt = blog.Excerpt ?? string.Empty,
                CoverImageUrl = blog.CoverImageUrl ?? string.Empty,
                Author = blog.Author ?? string.Empty,
                Views = blog.Views
            };

            // Partners and projects for layout
            ViewData["Partners"] = GetPartners();
            ViewData["Projects"] = GetProjects();
            ViewData["Title"] = post.Title;
            ViewData["BlogId"] = blog.Id; // Pass blog ID for tracking

            return View(post);
        }

        [HttpPost]
        [AllowAnonymous] // Add this attribute
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrackBlogView(string blogId, string? systemName)
        {
            if (string.IsNullOrEmpty(blogId))
            {
                return Json(new { success = false, message = "Blog ID is required" });
            }

            // Get client IP address
            var ipAddress = GetClientIpAddress();
            var result = await _blogService.TrackBlogViewAsync(blogId, ipAddress, systemName);

            return Json(result);
        }

        private string? GetClientIpAddress()
        {
            try
            {
                // Check for forwarded IP (when behind proxy/load balancer)
                var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    var ips = forwardedFor.Split(',');
                    if (ips.Length > 0)
                    {
                        return ips[0].Trim();
                    }
                }

                // Check for real IP
                var realIp = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp.Trim();
                }

                // Fallback to connection remote IP
                return HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IActionResult> ProjectDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var project = await _projectService.GetProjectDetailsAsync(id);
            if (project == null)
                return NotFound();

            ViewData["Partners"] = GetPartners();
            ViewData["Projects"] = GetProjects();
            ViewData["Title"] = project.Name;
            return View(project);
        }

        [HttpGet("Become-A-Vendor")]
        public IActionResult BecomeAVendor()
        {
            var model = new HomePageVM();
            model.Banner = _carouselService.GetAllCarouselsService().Where(x => x.PageType == CarouselPageType.Vendor && x.IsActive).ToList();
            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();

            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitVendorRegistration()
        {
            try
            {
                // Extract form data
                var model = new VendorRegistrationDto()
                {
                    CompanyName = Request.Form["companyName"].ToString(),
                    ContactPerson = Request.Form["contactPerson"].ToString(),
                    Email = Request.Form["email"].ToString(),
                    PhoneNumber = Request.Form["phoneNumber"].ToString(),
                    CACNumber = Request.Form["cacNumber"].ToString(),
                    TIN = Request.Form["tin"].ToString(),
                    BusinessCategory = Request.Form["businessCategory"].ToString(),
                    BusinessAddress = Request.Form["businessAddress"].ToString(),
                    File = Request.Form.Files["document"] as IFormFile
                };

                // Validate required fields
                if (string.IsNullOrWhiteSpace(model.CompanyName) || string.IsNullOrWhiteSpace(model.ContactPerson) || string.IsNullOrWhiteSpace(model.Email) 
                    || string.IsNullOrWhiteSpace(model.PhoneNumber) ||string.IsNullOrWhiteSpace(model.TIN) 
                    || string.IsNullOrWhiteSpace(model.BusinessCategory) ||string.IsNullOrWhiteSpace(model.BusinessAddress))
                {
                    return Json(new
                    {
                        isError = true,
                        message = "All required fields must be filled."
                    });
                }

                // Handle file upload
                if (model.File != null && model.File.Length > 0)
                {
                    // Validate file size (10MB)
                    if (model.File.Length > 10 * 1024 * 1024)
                    {
                        return Json(new
                        {
                            isError = true,
                            message = "Document file size exceeds 10MB limit."
                        });
                    }

                    // Validate file extension
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.File.FileName).ToLowerInvariant();

                    if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                    {
                        return Json(new
                        {
                            isError = true,
                            message = "Invalid file type. Only PDF, DOC, DOCX, JPG, JPEG, PNG, and GIF files are allowed."
                        });
                    }

                    // Create upload directory if it doesn't exist
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "vendor-documents");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueFileName = $"{Guid.NewGuid()}_{model.File.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }

                    model.FilePath = Path.Combine("uploads", "vendor-documents", uniqueFileName).Replace("\\", "/");
                }
                var res = await _vendorService.CreateVendorService(model);
                return Json(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vendor registration");

                return Json(new
                {
                    isError = true,
                    message = "An error occurred while processing your registration. Please try again."
                });
            }
        }

        [Authorize]
        public IActionResult Admin()
        {
            return RedirectToAction("Index", "Admin");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
   
    }
    
}

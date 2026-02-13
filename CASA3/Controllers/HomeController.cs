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

        public HomeController(ILogger<HomeController> logger, IVendorService vendorService, IAffiliateService affiliateService, INewsletterSubscriptionService newsletterSubscriptionService, IContactUsService contactUsService, IStaffService staffService, IProjectService projectService, INewsLetterService newsLetter, IPartnerService partnerService)
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
        }

        public IActionResult Index()
        {
            var model = new HomePageVM();
            model.Banner = new List<BannerDto>
            {
                new BannerDto
                {
                    ImageUrl = "/images/banners/banner-1.jpg",
                    Tag = "PROJECT OF THE MONTH",
                    Title = "CAPRI ISLAND",
                    Subtitle = "For those who dream in color",
                    CtaText = "DOWNLOAD BROCHURE",
                    CtaUrl = "#"
                },
                new BannerDto
                {
                    ImageUrl = "/images/banners/banner-2.jpg",
                    Tag = "FEATURED DEVELOPMENT",
                    Title = "CASA III",
                    Subtitle = "Luxury meets serenity",
                    CtaText = "VIEW PROJECT",
                    CtaUrl = "#"
                }
            };

            // Featured Projects data
            model.FeaturedProjects = new List<FeaturedProjectDto>
            {
                new FeaturedProjectDto { ImageUrl = "/images/projects/project-1.webp", Title = "CAPRI ISLAND", Url = "/projects/capri" },
                new FeaturedProjectDto { ImageUrl = "/images/projects/project-2.webp", Title = "LANGKAWI", Url = "/projects/langkawi" },
                new FeaturedProjectDto { ImageUrl = "/images/projects/project-3.webp", Title = "MALDIVES", Url = "/projects/maldives" }
            };

            // Bilaad Footprint data
            model.FootprintYears = new List<FootprintYearDto>
            {
                new FootprintYearDto
                {
                    Year = 2018,
                    Projects = new List<FootprintProjectDto>
                    {
                        new FootprintProjectDto
                        {
                            Title = "BOBOWASI ISLAND",
                            Subtitle = "Pioneer project of Bobowasi Island",
                            ImageUrl = "/images/projects/project-1.webp",
                            Url = "/projects/bobowasi"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BORABORA ISLAND",
                            Subtitle = "Pioneer project of Borabora Island",
                            ImageUrl = "/images/projects/project-2.webp",
                            Url = "/projects/borabora"
                        }
                    }
                },
                new FootprintYearDto { Year = 2019, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2020, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2021, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2022, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto
                {
                    Year = 2023,
                    Projects = new List<FootprintProjectDto>
                    {
                        new FootprintProjectDto
                        {
                            Title = "BOBOWASI ISLAND",
                            Subtitle = "Pioneer project of Bobowasi Island",
                            ImageUrl = "/images/projects/project-1.webp",
                            Url = "/projects/bobowasi"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BORABORA ISLAND",
                            Subtitle = "Pioneer project of Borabora Island",
                            ImageUrl = "/images/projects/project-2.webp",
                            Url = "/projects/borabora"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BOBOWASI ISLAND",
                            Subtitle = "Pioneer project of Bobowasi Island",
                            ImageUrl = "/images/projects/project-1.webp",
                            Url = "/projects/bobowasi"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BORABORA ISLAND",
                            Subtitle = "Pioneer project of Borabora Island",
                            ImageUrl = "/images/projects/project-2.webp",
                            Url = "/projects/borabora"
                        }
                    }
                },
                new FootprintYearDto { Year = 2024, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2025, Projects = new List<FootprintProjectDto>() }
            };
            model.SelectedFootprintYear = 2018;

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
            return new List<ProjectDto>
            {
                new ProjectDto { Id = "1", Name = "CAPRI ISLAND", Url = "/projects/capri" },
                new ProjectDto { Id = "2", Name = "EMERALD", Url = "/projects/emerald" },
                new ProjectDto { Id = "3", Name = "ONYX", Url = "/projects/onyx" },
                new ProjectDto { Id = "4", Name = "OPAL", Url = "/projects/opal" },
                new ProjectDto { Id = "5", Name = "AVENTURINE", Url = "/projects/aventurine" },
                new ProjectDto { Id = "6", Name = "AMAZON", Url = "/projects/amazon" },
                new ProjectDto { Id = "7", Name = "BAHAMAS", Url = "/projects/bahamas" },
                new ProjectDto { Id = "8", Name = "BIMINI", Url = "/projects/bimini" },
                new ProjectDto { Id = "9", Name = "BALI ISLAND", Url = "/projects/bali" },
                new ProjectDto { Id = "10", Name = "BARBADOS ISLAND", Url = "/projects/barbados" },
                new ProjectDto { Id = "11", Name = "BOBOWASI ISLAND", Url = "/projects/bobowasi" },
                new ProjectDto { Id = "12", Name = "BORA BORA ISLAND", Url = "/projects/borabora" },
                new ProjectDto { Id = "13", Name = "FIJI ISLAND", Url = "/projects/fiji" },
                new ProjectDto { Id = "14", Name = "LANGKAWI ISLAND", Url = "/projects/langkawi" },
                new ProjectDto { Id = "15", Name = "MALDIVES", Url = "/projects/maldives" },
                new ProjectDto { Id = "16", Name = "MAURITIUS ISLAND", Url = "/projects/mauritius" },
                new ProjectDto { Id = "17", Name = "SEYCHELLES", Url = "/projects/seychelles" },
                new ProjectDto { Id = "18", Name = "ZANZIBAR", Url = "/projects/zanzibar" }
            };
        }

        private List<PartnerVM> GetPartners()
        {
            return _partnerService.GetAllPartnersService();
        }

        public IActionResult AboutUs()
        {
            var model = new HomePageVM();

            var teamMembers = _staffService.GetAllStaffAsTeamMembers();
            if (teamMembers == null || !teamMembers.Any())
                teamMembers = GetAllTeamMembers();
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

            // Dummy blog posts
            var allBlogPosts = new List<BlogPostDto>
            {
                new BlogPostDto
                {
                    Id = 1,
                    Title = "BUYING YOUR FIRST HOME IN NIGERIA: A COMPLETE STEP-BY-STEP GUIDE.",
                    Slug = "buying-first-home-nigeria",
                    Category = "Real Estate Investment Guide",
                    PublishedDate = new DateTime(2025, 11, 11),
                    Content = "This comprehensive guide walks you through every step of purchasing your first home in Nigeria. Learn about the legal requirements, financial planning, property inspection, and documentation needed to make an informed decision.",
                    Excerpt = "A complete guide to buying your first property in Nigeria with expert tips and step-by-step instructions.",
                    CoverImageUrl = "/images/blog/blog-1.webp",
                    Author = "Dr. Emmanuel Bassi Usman",
                    Views = 1250
                },
                new BlogPostDto
                {
                    Id = 2,
                    Title = "THE FUTURE OF SUSTAINABLE REAL ESTATE DEVELOPMENT IN AFRICA",
                    Slug = "sustainable-real-estate-africa",
                    Category = "Market Trends",
                    PublishedDate = new DateTime(2025, 10, 28),
                    Content = "Explore the emerging trends in sustainable real estate development across Africa. Discover how green building practices, renewable energy integration, and community-focused development are reshaping the landscape.",
                    Excerpt = "An in-depth look at sustainability trends revolutionizing African real estate markets.",
                    CoverImageUrl = "/images/blog/blog-2.webp",
                    Author = "Abdulfatai Musa",
                    Views = 892
                },
                new BlogPostDto
                {
                    Id = 3,
                    Title = "PROPERTY INVESTMENT: MAXIMIZING RETURNS AND MINIMIZING RISKS",
                    Slug = "property-investment-strategies",
                    Category = "Investment Tips",
                    PublishedDate = new DateTime(2025, 09, 15),
                    Content = "Learn strategic approaches to real estate investment that can help you build wealth over time. This guide covers risk assessment, portfolio diversification, and long-term value creation.",
                    Excerpt = "Master the art of real estate investment with proven strategies for success.",
                    CoverImageUrl = "/images/blog/blog-3.webp",
                    Author = "Abdulkadir Abdulkadir",
                    Views = 756
                },
                new BlogPostDto
                {
                    Id = 4,
                    Title = "UNDERSTANDING THE NIGERIAN REAL ESTATE MARKET: A 2025 OVERVIEW",
                    Slug = "nigerian-real-estate-2025",
                    Category = "Market Analysis",
                    PublishedDate = new DateTime(2025, 08, 22),
                    Content = "Get an overview of the current state of the Nigerian real estate market. Understand key market indicators, growth opportunities, and factors influencing property values in 2025.",
                    Excerpt = "Comprehensive analysis of Nigeria's real estate market trends and opportunities.",
                    CoverImageUrl = "/images/blog/blog-4.webp",
                    Author = "Dr. Emmanuel Bassi Usman",
                    Views = 1124
                },
                new BlogPostDto
                {
                    Id = 5,
                    Title = "COMMERCIAL VS RESIDENTIAL PROPERTIES: WHICH IS RIGHT FOR YOU?",
                    Slug = "commercial-vs-residential",
                    Category = "Property Types",
                    PublishedDate = new DateTime(2025, 07, 10),
                    Content = "Compare the benefits and challenges of investing in commercial versus residential properties. Discover which option aligns best with your financial goals and investment timeline.",
                    Excerpt = "A detailed comparison to help you choose the right property investment type.",
                    CoverImageUrl = "/images/blog/blog-5.webp",
                    Author = "Mallam Halliru Sa'ad",
                    Views = 634
                },
                new BlogPostDto
                {
                    Id = 6,
                    Title = "THE ROLE OF TECHNOLOGY IN MODERN REAL ESTATE TRANSACTIONS",
                    Slug = "technology-real-estate",
                    Category = "Technology & Innovation",
                    PublishedDate = new DateTime(2025, 06, 18),
                    Content = "Discover how technology is transforming the real estate industry. From virtual property tours to blockchain-based transactions, explore innovations reshaping how we buy and sell properties.",
                    Excerpt = "How digital innovation is revolutionizing real estate transactions and property management.",
                    CoverImageUrl = "/images/blog/blog-6.webp",
                    Author = "Tech Innovation Team",
                    Views = 892
                }
            };

            // Filter by search query
            var filteredPosts = allBlogPosts;
            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredPosts = allBlogPosts
                    .Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || b.Category.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || b.Excerpt.Contains(search, StringComparison.OrdinalIgnoreCase))
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

        public IActionResult BlogDetails(int id)
        {
            // Create same dummy posts as in Blog action and find by id
            var allBlogPosts = new List<BlogPostDto>
            {
                new BlogPostDto { Id = 1, Title = "BUYING YOUR FIRST HOME IN NIGERIA: A COMPLETE STEP-BY-STEP GUIDE.", Slug = "buying-first-home-nigeria", Category = "Real Estate Investment Guide", PublishedDate = new DateTime(2025,11,11), Content = "This is a detailed dummy content used for blog details. Replace with real content later.", Excerpt = "A complete guide to buying your first property in Nigeria...", CoverImageUrl = "/images/blog/blog-1.webp", Author = "Dr. Emmanuel Bassi Usman", Views = 1250 },
                new BlogPostDto { Id = 2, Title = "THE FUTURE OF SUSTAINABLE REAL ESTATE DEVELOPMENT IN AFRICA", Slug = "sustainable-real-estate-africa", Category = "Market Trends", PublishedDate = new DateTime(2025,10,28), Content = "This is a detailed dummy content used for blog details. Replace with real content later.", Excerpt = "An in-depth look at sustainability trends...", CoverImageUrl = "/images/blog/blog-2.webp", Author = "Abdulfatai Musa", Views = 892 },
                new BlogPostDto { Id = 3, Title = "PROPERTY INVESTMENT: MAXIMIZING RETURNS AND MINIMIZING RISKS", Slug = "property-investment-strategies", Category = "Investment Tips", PublishedDate = new DateTime(2025,9,15), Content = "This is a detailed dummy content used for blog details. Replace with real content later.", Excerpt = "Master the art of real estate investment...", CoverImageUrl = "/images/blog/blog-3.webp", Author = "Abdulkadir Abdulkadir", Views = 756 },
                new BlogPostDto { Id = 4, Title = "UNDERSTANDING THE NIGERIAN REAL ESTATE MARKET: A 2025 OVERVIEW", Slug = "nigerian-real-estate-2025", Category = "Market Analysis", PublishedDate = new DateTime(2025,8,22), Content = "This is a detailed dummy content used for blog details. Replace with real content later.", Excerpt = "Comprehensive analysis of Nigeria's real estate market...", CoverImageUrl = "/images/blog/blog-4.webp", Author = "Dr. Emmanuel Bassi Usman", Views = 1124 },
                new BlogPostDto { Id = 5, Title = "COMMERCIAL VS RESIDENTIAL PROPERTIES: WHICH IS RIGHT FOR YOU?", Slug = "commercial-vs-residential", Category = "Property Types", PublishedDate = new DateTime(2025,7,10), Content = "This is a detailed dummy content used for blog details. Replace with real content later.", Excerpt = "A detailed comparison to help you choose the right property...", CoverImageUrl = "/images/blog/blog-5.webp", Author = "Mallam Halliru Sa'ad", Views = 634 },
                new BlogPostDto { Id = 6, Title = "THE ROLE OF TECHNOLOGY IN MODERN REAL ESTATE TRANSACTIONS", Slug = "technology-real-estate", Category = "Technology & Innovation", PublishedDate = new DateTime(2025,6,18), Content = "This is a detailed dummy content used for blog details. Replace with real content later.", Excerpt = "How digital innovation is revolutionizing real estate...", CoverImageUrl = "/images/blog/blog-6.webp", Author = "Tech Innovation Team", Views = 892 }
            };

            var post = allBlogPosts.FirstOrDefault(p => p.Id == id) ?? allBlogPosts.First();

            // Partners and projects for layout
            ViewData["Partners"] = GetPartners();
            ViewData["Projects"] = GetProjects();
            ViewData["Title"] = post.Title;

            return View(post);
        }

        public async Task<IActionResult> ProjectDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var project = await _projectService.GetProjectDetailsAsync(id);
            if (project != null)
            {
                ViewData["Partners"] = GetPartners();
                ViewData["Projects"] = GetProjects();
                ViewData["Title"] = project.Name;
                return View(project);
            }

            // Fallback dummy data when project not in DB
            int numericId;
            if (!int.TryParse(id, out numericId))
                return NotFound();

            var dummyProjects = new List<ProjectDetailsDto>
            {
                new ProjectDetailsDto
                {
                    Id = "1",
                    Name = "CAPRI ISLAND",
                    HeroImageUrl = "/images/projects/project-1.webp",
                    Description = "Spanning 210 hectares in Kalantan island, Abiaj Longterm Island is a refined residential estate offering 10 thoughtfully-planned homes. From elegant 5-bedroom mansions to the charm of 4-bedroom duplexes, each home is designed with meticulous attention to detail while featuring top-notch aesthetics and security. Long-term island delivers a luxurious lifestyle where every detail is built to elevate everyday living.",
                    BrochurePdfUrl = "/files/capri-brochure.pdf",
                    BuildingDesigns = new List<BuildingDesignDto>()
                },
                new ProjectDetailsDto
                {
                    Id = "2",
                    Name = "EMERALD",
                    HeroImageUrl = "/images/projects/emerald-hero.webp",
                    Description = "The Emerald is a majestically designed five-bedroom villa that embodies refined living spaces with the perfect balance of contemporary luxe and elegant details. This distinctively crafted villa features an elegant guest quarters and an exquisite guard house. Balconies with sweeping views every detail has been crafted for comfort and sophistication. The finessed design and fixtures suitable for those who crave style, serenity.",
                    BrochurePdfUrl = "/files/emerald-brochure.pdf",
                    BuildingDesigns = new List<BuildingDesignDto>()
                },
                new ProjectDetailsDto
                {
                    Id = "3",
                    Name = "ONYX",
                    HeroImageUrl = "/images/projects/onyx-hero.webp",
                    Description = "This beautifully designed villa is a perfect blend of creativity and craftsmanship, emphasizing quality and comfort. It features five spacious generously suited two bedroom en-suite and an ultra-modern kitchen. The home is intelligently designed to offer a refined lifestyle and architectural opulence. Every space tells a story of sophistication with quality finishes and design excellence.",
                    BrochurePdfUrl = "/files/onyx-brochure.pdf",
                    BuildingDesigns = new List<BuildingDesignDto>()
                },
                new ProjectDetailsDto
                {
                    Id = "4",
                    Name = "OPAL",
                    HeroImageUrl = "/images/projects/opal-hero.webp",
                    Description = "This beautifully designed townhouse showcase a perfect blend of creativity and meticulous craftsmanship, focusing on quality and comfort. With luxury spaces and suite bedrooms and an employees rest area with quality finishing throughout. Adventurine redefines the modern living experience that meets the needs of today's families seeking style, functionality and premium comfort and details.",
                    BrochurePdfUrl = "/files/opal-brochure.pdf",
                    BuildingDesigns = new List<BuildingDesignDto>()
                },
                new ProjectDetailsDto
                {
                    Id = "5",
                    Name = "AVENTURINE",
                    HeroImageUrl = "/images/projects/aventurine-hero.webp",
                    Description = "Aventurine showcases three spacious en-suite bedrooms along with a contemporary lounge. All interior surfaces are enhanced by festive liquors that color in your mood-making interior and create both warmth and relaxed feelings. Whi exquisite details and high quality finishes throughout Aventurine redefines the ultimate lifestyle where every corner is built for comfort.",
                    BrochurePdfUrl = "/files/aventurine-brochure.pdf",
                    BuildingDesigns = new List<BuildingDesignDto>()
                }
            };

            var fallback = dummyProjects.FirstOrDefault(p => p.Id == id) ?? dummyProjects.First();
            var rand = new Random();
            int numberOfDesigns = rand.Next(1, 6);
            for (int i = 1; i <= numberOfDesigns; i++)
            {
                fallback.BuildingDesigns.Add(new BuildingDesignDto
                {
                    Id = i.ToString(),
                    Name = $"{fallback.Name} - Design Type {i}",
                    Description = "This beautifully designed building showcases a perfect blend of modern architecture and luxury living. The unit features spacious living areas, state-of-the-art amenities, and exquisite finishes throughout.",
                    ImageUrl = "/images/sample-project-details.webp",
                    FloorPlanPdfUrl = $"/files/floorplan-{i}.pdf"
                });
            }

            ViewData["Partners"] = GetPartners();
            ViewData["Projects"] = GetProjects();
            ViewData["Title"] = fallback.Name;
            return View(fallback);
        }

        [HttpGet("Become-A-Vendor")]
        public IActionResult BecomeAVendor()
        {
            var model = new HomePageVM();
            model.Banner = new List<BannerDto>
            {
                new BannerDto
                {
                    ImageUrl = "/images/banners/vbanner-1.jpg",
                    Tag = null,
                    Title = "Streamlined Vendor Management",
                    Subtitle = "Connect with procurement opportunities and manage vendor relationships efficiently",
                    CtaText = "Get Started",
                    CtaUrl = "#"
                },
                new BannerDto
                {
                    ImageUrl = "/images/banners/vbanner-2.jpg",
                    Tag = null,
                    Title = "Transparent Bidding Process",
                    Subtitle = "Access real-time bid opportunities with complete transparency and fairness",
                    CtaText = "View Opportunities",
                    CtaUrl = "#"
                }
            };
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
   
        private List<TeamMemberDto> GetAllTeamMembers()
        {
            var dummy = @"{Name} is a multidisciplinary business executive and real estate professional whose career is anchored in a strong engineering foundation and shaped by extensive experience in finance, risk management, and strategic operations. He is recognized for driving innovation and value creation across real estate development, financial services, and enterprise transformation.
            Trained as a Mechanical Engineer, Fawaz began his professional journey with a technical and systems-driven mindset that continues to influence his structured approach to problem-solving and execution. He later transitioned into the financial services sector, where he built deep expertise in underwriting, risk management, and portfolio analysis within Nigeria?s insurance industry.
            At Consolidated Hallmark Insurance Plc, Fawaz progressed across underwriting and risk management roles, contributing to complex risk assessments, wealth management, and strategic decision-making. This phase of his career strengthened his financial discipline and sharpened his ability to manage risk at scale.
            Building on this experience, Fawaz went on to co-found Harbourage Ltd, where he served as Chief Financial Officer, leading financial strategy and operational optimization with a strong focus on process improvement and scalable growth. He is a certified Financial Modeling and Valuation Analyst (FMVA?), a member of the Chartered Insurance Institute of Nigeria (CIIN), currently completing an MBA in Finance and Investment at Ahmadu Bello University, and a Harvard alumnus in Resilient Leadership, reflecting his commitment to world-class leadership and long-term value creation.
            Driven by impact, Fawaz continues to bridge global best practices with local market realities, shaping sustainable businesses, transformative real estate solutions, and future-ready enterprises across Nigeria and beyond.
                ";
            var teamMembers = new List<TeamMemberDto>
            {
                new TeamMemberDto
                {
                    Id = "1",
                    Name = "DR. SADIQ SULEIMAN ABDULLAHI",
                    Position = "CHAIRMAN, BOARD OF DIRECTORS",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/dr-sadiq-suleiman-abdullahi-668ab9300",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.BOD
                },
                new TeamMemberDto
                {
                    Id = "2",
                    Name = "ALIYU ALIYU",
                    Position = "EXECUTIVE DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/aliyu-aliyu",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.BOD
                },
                new TeamMemberDto
                {
                    Id = "3",
                    Name = "YAHYA AHMAD RUFAI",
                    Position = "NON-EXECUTIVE DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/yahya-ahmad-rufai",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.BOD
                },
                new TeamMemberDto
                {
                    Id = "4",
                    Name = "ROSS OLUYEDE",
                    Position = "INDEPENDENT DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/ross-oluyede",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.BOD
                },
                new TeamMemberDto
                {
                    Id = "5",
                    Name = "MALLAM HALLIRU SA'AD MALAMI",
                    Position = "INDEPENDENT DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/mallam-halliru",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.BOD
                },
                new TeamMemberDto
                {
                    Id = "6",
                    Name = "DR. NURATU MUSA ABDULLAHI",
                    Position = "INDEPENDENT DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/dr-nuratu-musa",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.BOD
                },
                new TeamMemberDto
                {
                    Id = "7",
                    Name = "DR. EMMANUEL BASSI USMAN",
                    Position = "CHIEF EXECUTIVE OFFICER",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/dr-emmanuel-bassi-usman",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.MGT
                },
                new TeamMemberDto
                {
                    Id = "8",
                    Name = "ABDULFATAI MUSA",
                    Position = "CHIEF OPERATIONS OFFICER",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/abdulfatai-musa",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.MGT
                },
                new TeamMemberDto
                {
                    Id = "9",
                    Name = "ABDULKADIR ABDULKADIR",
                    Position = "CHIEF BUSINESS AND STRATEGY OFFICER",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/abdulkadir-abdulkadir",
                    MemberInfo = dummy,
                    Category = TeamMemeberCategory.MGT
                }
            };

            return teamMembers;
        }
    }

    
}

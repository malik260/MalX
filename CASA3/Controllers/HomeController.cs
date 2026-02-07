using CASA3.Models;
using Core.DTOs;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CASA3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            model.Newsletters = new List<NewsletterDto>
            {
                new NewsletterDto
                {
                    CoverImageUrl = "/images/Newsletter/Newsletter-Feb.-2026-Cover.jpg-scaled.webp",
                    PdfUrl = "#" // Replace with actual PDF URL
                },
                new NewsletterDto
                {
                    CoverImageUrl = "/images/Newsletter/Newsletter-Jan.-2026-Cover-scaled.webp",
                    PdfUrl = "#" // Replace with actual PDF URL
                },
                new NewsletterDto
                {
                    CoverImageUrl = "/images/Newsletter/COVER-1_page-0001-scaled.webp",
                    PdfUrl = "#" // Replace with actual PDF URL
                }
            };

            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();
            
            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            return View(model);
        }

        private List<ProjectDto> GetProjects()
        {
            return new List<ProjectDto>
            {
                new ProjectDto { Name = "AMAZON", Url = "/projects/amazon" },
                new ProjectDto { Name = "BAHAMAS", Url = "/projects/bahamas" },
                new ProjectDto { Name = "BIMINI", Url = "/projects/bimini" },
                new ProjectDto { Name = "BALI ISLAND", Url = "/projects/bali" },
                new ProjectDto { Name = "BARBADOS ISLAND", Url = "/projects/barbados" },
                new ProjectDto { Name = "BOBOWASI ISLAND", Url = "/projects/bobowasi" },
                new ProjectDto { Name = "BORA BORA ISLAND", Url = "/projects/borabora" },
                new ProjectDto { Name = "CAPRI ISLAND", Url = "/projects/capri" },
                new ProjectDto { Name = "FIJI ISLAND", Url = "/projects/fiji" },
                new ProjectDto { Name = "LANGKAWI ISLAND", Url = "/projects/langkawi" },
                new ProjectDto { Name = "MALDIVES", Url = "/projects/maldives" },
                new ProjectDto { Name = "MAURITIUS ISLAND", Url = "/projects/mauritius" },
                new ProjectDto { Name = "SEYCHELLES", Url = "/projects/seychelles" },
                new ProjectDto { Name = "ZANZIBAR", Url = "/projects/zanzibar" }
            };
        }

        private List<PartnerDto> GetPartners()
        {
            return new List<PartnerDto>
            {
                new PartnerDto
                {
                    Name = "Jaiz Bank",
                    ImageUrl = "/images/partners/jaiz-1-1.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "Metropolitan",
                    ImageUrl = "/images/partners/metro-2.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "Partner 3",
                    ImageUrl = "/images/partners/My-project-1-9-1-1.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "New Edge Bank",
                    ImageUrl = "/images/partners/117-1-1.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "Metropolitan",
                    ImageUrl = "/images/partners/metro-2.webp",
                    WebsiteUrl = "#"
                }
            };
        }

        public IActionResult AboutUs()
        {
            var model = new HomePageVM();
            
            // Board of Directors data
            model.BoardOfDirectors = new List<TeamMemberDto>
            {
                new TeamMemberDto
                {
                    Id = 1,
                    Name = "DR. SADIQ SULEIMAN ABDULLAHI",
                    Position = "CHAIRMAN, BOARD OF DIRECTORS",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/dr-sadiq-suleiman-abdullahi-668ab9300"
                },
                new TeamMemberDto
                {
                    Id = 2,
                    Name = "ALIYU ALIYU",
                    Position = "EXECUTIVE DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/aliyu-aliyu"
                },
                new TeamMemberDto
                {
                    Id = 3,
                    Name = "YAHYA AHMAD RUFAI",
                    Position = "NON-EXECUTIVE DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/yahya-ahmad-rufai"
                },
                new TeamMemberDto
                {
                    Id = 4,
                    Name = "ROSS OLUYEDE",
                    Position = "INDEPENDENT DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/ross-oluyede"
                },
                new TeamMemberDto
                {
                    Id = 5,
                    Name = "MALLAM HALLIRU SA'AD MALAMI",
                    Position = "INDEPENDENT DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/mallam-halliru"
                },
                new TeamMemberDto
                {
                    Id = 6,
                    Name = "DR. NURATU MUSA ABDULLAHI",
                    Position = "INDEPENDENT DIRECTOR",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/dr-nuratu-musa"
                }
            };
            
            // Management Team data
            model.ManagementTeam = new List<TeamMemberDto>
            {
                new TeamMemberDto
                {
                    Id = 1,
                    Name = "DR. EMMANUEL BASSI USMAN",
                    Position = "CHIEF EXECUTIVE OFFICER",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/dr-emmanuel-bassi-usman"
                },
                new TeamMemberDto
                {
                    Id = 2,
                    Name = "ABDULFATAI MUSA",
                    Position = "CHIEF OPERATIONS OFFICER",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/abdulfatai-musa"
                },
                new TeamMemberDto
                {
                    Id = 3,
                    Name = "ABDULKADIR ABDULKADIR",
                    Position = "CHIEF BUSINESS AND STRATEGY OFFICER",
                    ImageUrl = "/images/dummy user.jpg",
                    LinkedInUrl = "https://linkedin.com/in/abdulkadir-abdulkadir"
                }
            };
            
            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();
            
            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();
            
            ViewData["Title"] = "About Us";
            
            return View(model);
        }
        [HttpPost]
        public IActionResult ContactUs(ContactFormDto form)
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

            // TODO: persist enquiry / send email - placeholder for now

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Thank you for contacting us. We'll be in touch shortly." });
            }

            TempData["ContactSuccess"] = "Thank you for contacting us. We'll be in touch shortly.";
            return RedirectToAction("ContactUs");
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
        public IActionResult SubscribeNewsletter(NewsletterSubscriptionDto subscription)
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

            // TODO: Save to database or send email confirmation

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Thank you for subscribing! Check your email for confirmation." });
            }

            TempData["NewsletterSuccess"] = "Thank you for subscribing! Check your email for confirmation.";
            return RedirectToAction("Index");
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

            model.Pagination = new Core.Models.PaginationModel
            {
                TotalItems = totalItems,
                ItemsPerPage = pageSize,
                CurrentPage = page,
                SearchQuery = search ?? ""
            };

            // Apply pagination using Skip and Take
            model.BlogPosts = filteredPosts
                .OrderByDescending(b => b.PublishedDate)
                .Skip(model.Pagination.SkipCount)
                .Take(pageSize)
                .ToList();

            // Recent posts - Get the 3 most recent from all posts (not filtered)
            model.RecentBlogPosts = allBlogPosts
                .OrderByDescending(b => b.PublishedDate)
                .Take(3)
                .ToList();

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

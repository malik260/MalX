using Core.DTOs;
using Core.Enum;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly IProjectService _projectService;
        private readonly IMediaService _mediaService;
        private readonly IVendorService _vendorService;
        private readonly IAffiliateService _affiliateService;
        private readonly IContactUsService _contactUsService;
        private readonly INewsletterSubscriptionService _newsletterSubscriptionService;
        private readonly IWebHostEnvironment _env;

        public AdminController(IStaffService staffService, IProjectService projectService, IMediaService mediaService, IVendorService vendorService, IAffiliateService affiliateService, IContactUsService contactUsService, INewsletterSubscriptionService newsletterSubscriptionService, IWebHostEnvironment env)
        {
            _staffService = staffService;
            _projectService = projectService;
            _mediaService = mediaService;
            _vendorService = vendorService;
            _affiliateService = affiliateService;
            _contactUsService = contactUsService;
            _newsletterSubscriptionService = newsletterSubscriptionService;
            _env = env;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            var staff = _staffService.GetAllStaffAsTeamMembers();
            var projects = _projectService.GetAllProjects();
            var vendors = _vendorService.GetAllRegisteredVendorsService();
            var affiliates = _affiliateService.GetAllRegisteredAffiliatesService();
            var contacts = _contactUsService.GetAllContactUsService();
            var newsletters = _newsletterSubscriptionService.GetAllNewsletterSubscriptionsService();
            var model = new DashboardVM
            {
                StaffCount = staff?.Count ?? 0,
                ProjectCount = projects?.Count ?? 0,
                VendorCount = vendors?.Count ?? 0,
                AffiliateCount = affiliates?.Count ?? 0,
                ContactCount = contacts?.Count ?? 0,
                NewsletterCount = newsletters?.Count ?? 0
            };
            return View(model);
        }

        public IActionResult Staffs()
        {
            var list = _staffService.GetAllStaffAsTeamMembers();
            ViewData["Title"] = "Staff Setup";
            return View(list);
        }

        [HttpGet]
        public IActionResult AddStaff()
        {
            ViewData["Title"] = "Add Staff";
            return View(new StaffCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStaff(StaffCreateDto model, IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Position))
            {
                ModelState.AddModelError(string.Empty, "Staff name and role (position) are required.");
                return View(model);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Invalid image type. Allowed: JPG, PNG, GIF, WebP.");
                    return View(model);
                }
                if (imageFile.Length > 5 * 1024 * 1024) // 5MB
                {
                    ModelState.AddModelError(string.Empty, "Image size must be 5MB or less.");
                    return View(model);
                }

                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "staff");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await imageFile.CopyToAsync(stream);
                model.ImageUrl = $"/uploads/staff/{fileName}";
            }

            var result = await _staffService.CreateStaffAsync(model);
            if (result.success)
            {
                TempData["StaffMessage"] = result.Message;
                return RedirectToAction(nameof(Staffs));
            }
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStaff(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Staffs));
            var dto = await _staffService.GetStaffByIdAsync(id);
            if (dto == null)
            {
                TempData["StaffError"] = "Staff not found.";
                return RedirectToAction(nameof(Staffs));
            }
            var model = new StaffCreateDto
            {
                Name = dto.Name,
                Position = dto.Position,
                ImageUrl = dto.ImageUrl,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                LinkedInUrl = dto.LinkedInUrl,
                MemberInfo = dto.MemberInfo,
                Category = dto.Category
            };
            ViewData["Title"] = "Edit Staff";
            ViewData["StaffId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(string id, StaffCreateDto model, IFormFile? imageFile)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Staffs));
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Position))
            {
                ModelState.AddModelError(string.Empty, "Staff name and role (position) are required.");
                ViewData["StaffId"] = id;
                return View(model);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Invalid image type. Allowed: JPG, PNG, GIF, WebP.");
                    ViewData["StaffId"] = id;
                    return View(model);
                }
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(string.Empty, "Image size must be 5MB or less.");
                    ViewData["StaffId"] = id;
                    return View(model);
                }

                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "staff");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await imageFile.CopyToAsync(stream);
                model.ImageUrl = $"/uploads/staff/{fileName}";
            }
            else
            {
                var current = await _staffService.GetStaffByIdAsync(id);
                if (current != null)
                    model.ImageUrl = current.ImageUrl;
            }

            var result = await _staffService.UpdateStaffAsync(id, model);
            if (result.success)
            {
                TempData["StaffMessage"] = result.Message;
                return RedirectToAction(nameof(Staffs));
            }
            ModelState.AddModelError(string.Empty, result.Message);
            ViewData["StaffId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Staffs));
            var result = await _staffService.DeleteStaffAsync(id);
            TempData["StaffMessage"] = result.Message;
            return RedirectToAction(nameof(Staffs));
        }

        public IActionResult Projects()
        {
            var list = _projectService.GetAllProjects();
            ViewData["Title"] = "Projects";
            return View(list);
        }

        [HttpGet]
        public IActionResult AddProject()
        {
            ViewData["Title"] = "Add Project";
            return View(new ProjectCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProject(ProjectCreateDto model, IFormFile? heroImageFile, IFormFile? brochurePdfFile)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Project name is required.");
                return View(model);
            }
            var result = await _projectService.CreateProjectAsync(model);
            if (!result.success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
            var projectId = (result.Data as ProjectDto)?.Id;
            if (!string.IsNullOrEmpty(projectId))
            {
                if (heroImageFile != null && heroImageFile.Length > 0)
                {
                    var path = await SaveMediaFileAsync(heroImageFile, "Hero", "Project", projectId);
                    if (path != null) model.HeroImageUrl = path;
                }
                if (brochurePdfFile != null && brochurePdfFile.Length > 0)
                {
                    var path = await SaveMediaFileAsync(brochurePdfFile, "Brochure", "Project", projectId);
                    if (path != null) model.BrochurePdfUrl = path;
                }
                if (model.HeroImageUrl != null || model.BrochurePdfUrl != null)
                    await _projectService.UpdateProjectAsync(projectId, model);
            }
            TempData["ProjectMessage"] = result.Message;
            return RedirectToAction(nameof(Projects));
        }

        [HttpGet]
        public async Task<IActionResult> EditProject(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Projects));
            var dto = await _projectService.GetProjectByIdAsync(id);
            if (dto == null)
            {
                TempData["ProjectError"] = "Project not found.";
                return RedirectToAction(nameof(Projects));
            }
            var model = new ProjectCreateDto
            {
                Name = dto.Name,
                Url = dto.Url,
                Slug = dto.Slug,
                Description = dto.Description,
                HeroImageUrl = dto.HeroImageUrl,
                BrochurePdfUrl = dto.BrochurePdfUrl,
                Year = dto.Year,
                IsFeatured = dto.IsFeatured,
                Category = dto.Category
            };
            ViewData["Title"] = "Edit Project";
            ViewData["ProjectId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject(string id, ProjectCreateDto model, IFormFile? heroImageFile, IFormFile? brochurePdfFile)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Projects));
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Project name is required.");
                ViewData["ProjectId"] = id;
                return View(model);
            }
            if (heroImageFile != null && heroImageFile.Length > 0)
            {
                var path = await SaveMediaFileAsync(heroImageFile, "Hero", "Project", id);
                if (path != null) model.HeroImageUrl = path;
            }
            if (brochurePdfFile != null && brochurePdfFile.Length > 0)
            {
                var path = await SaveMediaFileAsync(brochurePdfFile, "Brochure", "Project", id);
                if (path != null) model.BrochurePdfUrl = path;
            }
            var result = await _projectService.UpdateProjectAsync(id, model);
            if (result.success)
            {
                TempData["ProjectMessage"] = result.Message;
                return RedirectToAction(nameof(Projects));
            }
            ModelState.AddModelError(string.Empty, result.Message);
            ViewData["ProjectId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Projects));
            var result = await _projectService.DeleteProjectAsync(id);
            TempData["ProjectMessage"] = result.Message;
            return RedirectToAction(nameof(Projects));
        }

        public IActionResult ProjectUnits(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return RedirectToAction(nameof(Projects));
            var project = _projectService.GetProjectByIdAsync(projectId).GetAwaiter().GetResult();
            if (project == null)
            {
                TempData["ProjectError"] = "Project not found.";
                return RedirectToAction(nameof(Projects));
            }
            var units = _projectService.GetBuildingDesignsByProjectId(projectId);
            ViewData["Title"] = $"Units — {project.Name}";
            ViewData["ProjectId"] = projectId;
            ViewData["ProjectName"] = project.Name;
            return View(units);
        }

        [HttpGet]
        public IActionResult AddUnit(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return RedirectToAction(nameof(Projects));
            ViewData["Title"] = "Add Unit";
            ViewData["ProjectId"] = projectId;
            return View(new BuildingDesignCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUnit(string projectId, BuildingDesignCreateDto model, IFormFile? imageFile, IFormFile? brochurePdfFile, IFormFile? floorPlanPdfFile)
        {
            if (string.IsNullOrEmpty(projectId))
                return RedirectToAction(nameof(Projects));
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Unit name is required.");
                ViewData["ProjectId"] = projectId;
                return View(model);
            }
            var result = await _projectService.CreateBuildingDesignAsync(projectId, model);
            if (!result.success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                ViewData["ProjectId"] = projectId;
                return View(model);
            }
            var unitId = (result.Data as BuildingDesignDto)?.Id;
            if (!string.IsNullOrEmpty(unitId))
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var path = await SaveMediaFileAsync(imageFile, "Image", "BuildingDesign", unitId);
                    if (path != null) model.ImageUrl = path;
                }
                if (brochurePdfFile != null && brochurePdfFile.Length > 0)
                {
                    var path = await SaveMediaFileAsync(brochurePdfFile, "Brochure", "BuildingDesign", unitId);
                    if (path != null) model.BrochurePdfUrl = path;
                }
                if (floorPlanPdfFile != null && floorPlanPdfFile.Length > 0)
                {
                    var path = await SaveMediaFileAsync(floorPlanPdfFile, "FloorPlan", "BuildingDesign", unitId);
                    if (path != null) model.FloorPlanPdfUrl = path;
                }
                if (model.ImageUrl != null || model.BrochurePdfUrl != null || model.FloorPlanPdfUrl != null)
                    await _projectService.UpdateBuildingDesignAsync(unitId, model);
            }
            TempData["UnitMessage"] = result.Message;
            return RedirectToAction(nameof(ProjectUnits), new { projectId });
        }

        [HttpGet]
        public async Task<IActionResult> EditUnit(string projectId, string id)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Projects));
            var design = await _projectService.GetBuildingDesignByIdAsync(id);
            if (design == null || design.Id == null)
            {
                TempData["UnitError"] = "Unit not found.";
                return RedirectToAction(nameof(ProjectUnits), new { projectId });
            }
            var model = new BuildingDesignCreateDto
            {
                Name = design.Name,
                Description = design.Description,
                Location = design.Location,
                ImageUrl = design.ImageUrl,
                BrochurePdfUrl = design.BrochurePdfUrl,
                FloorPlanPdfUrl = design.FloorPlanPdfUrl
            };
            ViewData["Title"] = "Edit Unit";
            ViewData["ProjectId"] = projectId;
            ViewData["UnitId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUnit(string projectId, string id, BuildingDesignCreateDto model, IFormFile? imageFile, IFormFile? brochurePdfFile, IFormFile? floorPlanPdfFile)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Projects));
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Unit name is required.");
                ViewData["ProjectId"] = projectId;
                ViewData["UnitId"] = id;
                return View(model);
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                var path = await SaveMediaFileAsync(imageFile, "Image", "BuildingDesign", id);
                if (path != null) model.ImageUrl = path;
            }
            if (brochurePdfFile != null && brochurePdfFile.Length > 0)
            {
                var path = await SaveMediaFileAsync(brochurePdfFile, "Brochure", "BuildingDesign", id);
                if (path != null) model.BrochurePdfUrl = path;
            }
            if (floorPlanPdfFile != null && floorPlanPdfFile.Length > 0)
            {
                var path = await SaveMediaFileAsync(floorPlanPdfFile, "FloorPlan", "BuildingDesign", id);
                if (path != null) model.FloorPlanPdfUrl = path;
            }
            var result = await _projectService.UpdateBuildingDesignAsync(id, model);
            if (result.success)
            {
                TempData["UnitMessage"] = result.Message;
                return RedirectToAction(nameof(ProjectUnits), new { projectId });
            }
            ModelState.AddModelError(string.Empty, result.Message);
            ViewData["ProjectId"] = projectId;
            ViewData["UnitId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUnit(string projectId, string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Projects));
            var result = await _projectService.DeleteBuildingDesignAsync(id);
            TempData["UnitMessage"] = result.Message;
            if (!string.IsNullOrEmpty(projectId))
                return RedirectToAction(nameof(ProjectUnits), new { projectId });
            return RedirectToAction(nameof(Projects));
        }

        // ───────── Contact Us ─────────

        public IActionResult Contacts()
        {
            var list = _contactUsService.GetAllContactUsService();
            ViewData["Title"] = "Contact Us";
            return View(list);
        }

        // ───────── Vendor Management ─────────

        public IActionResult Vendors()
        {
            var list = _vendorService.GetAllRegisteredVendorsService();
            ViewData["Title"] = "Vendor Management";
            return View(list);
        }

        [HttpGet]
        public IActionResult AddVendor()
        {
            ViewData["Title"] = "Add Vendor";
            return View(new VendorRegistrationDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVendor(VendorRegistrationDto model, IFormFile? file)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName) || string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Company name and email are required.");
                return View(model);
            }

            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "vendors");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);
                model.FilePath = $"/uploads/vendors/{fileName}";
            }

            var result = await _vendorService.CreateVendorService(model);
            if (result.success)
            {
                TempData["VendorMessage"] = result.Message;
                return RedirectToAction(nameof(Vendors));
            }
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditVendor(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Vendors));
            var vendor = await _vendorService.GetVendorIdMain(id);
            if (vendor == null)
            {
                TempData["VendorError"] = "Vendor not found.";
                return RedirectToAction(nameof(Vendors));
            }
            var model = new VendorUpdateDto
            {
                Id = vendor.Id,
                CompanyName = vendor.CompanyName,
                ContactPerson = vendor.ContactPerson,
                Email = vendor.Email,
                PhoneNumber = vendor.PhoneNumber,
                CACNumber = vendor.CACNumber,
                TIN = vendor.TIN,
                BusinessCategory = vendor.BusinessCategory,
                BusinessAddress = vendor.BusinessAddress,
                FilePath = vendor.FilePath,
                Status = vendor.Status
            };
            ViewData["Title"] = "Edit Vendor";
            ViewData["VendorId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVendor(string id, VendorUpdateDto model, IFormFile? file)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Vendors));
            model.Id = id;

            if (string.IsNullOrWhiteSpace(model.CompanyName) || string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Company name and email are required.");
                ViewData["VendorId"] = id;
                return View(model);
            }

            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "vendors");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(stream);
                model.FilePath = $"/uploads/vendors/{fileName}";
            }
            else
            {
                var current = await _vendorService.GetVendorIdMain(id);
                if (current != null)
                    model.FilePath = current.FilePath;
            }

            var result = await _vendorService.UpdateStudentService(model);
            if (result.success)
            {
                TempData["VendorMessage"] = result.Message;
                return RedirectToAction(nameof(Vendors));
            }
            ModelState.AddModelError(string.Empty, result.Message);
            ViewData["VendorId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVendor(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Vendors));
            var result = await _vendorService.DeleteVendorByIdService(id);
            TempData["VendorMessage"] = result.Message;
            return RedirectToAction(nameof(Vendors));
        }

        // ───────── Affiliate Management ─────────

        public IActionResult Affiliates()
        {
            var list = _affiliateService.GetAllRegisteredAffiliatesService();
            ViewData["Title"] = "Affiliate Management";
            return View(list);
        }

        [HttpGet]
        public IActionResult AddAffiliate()
        {
            ViewData["Title"] = "Add Affiliate";
            return View(new AffiliateRegistrationDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAffiliate(AffiliateRegistrationDto model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError(string.Empty, "First name, last name, email and phone are required.");
                return View(model);
            }

            var result = await _affiliateService.CreateAffiliateService(model);
            if (result.success)
            {
                TempData["AffiliateMessage"] = result.Message;
                return RedirectToAction(nameof(Affiliates));
            }
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditAffiliate(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Affiliates));
            var affiliate = await _affiliateService.GetAffiliateIdMain(id);
            if (affiliate == null)
            {
                TempData["AffiliateError"] = "Affiliate not found.";
                return RedirectToAction(nameof(Affiliates));
            }
            var model = new AffiliateUpdateDto
            {
                Id = affiliate.Id,
                FirstName = affiliate.FirstName,
                LastName = affiliate.LastName,
                Email = affiliate.Email,
                Phone = affiliate.Phone,
                StreetAddress = affiliate.StreetAddress,
                StateProvince = affiliate.StateProvince,
                Country = affiliate.Country,
                AccountName = affiliate.AccountName,
                BankName = affiliate.BankName,
                AccountNumber = affiliate.AccountNumber,
                Status = affiliate.Status
            };
            ViewData["Title"] = "Edit Affiliate";
            ViewData["AffiliateId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAffiliate(string id, AffiliateUpdateDto model)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Affiliates));
            model.Id = id;

            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError(string.Empty, "First name, last name, email and phone are required.");
                ViewData["AffiliateId"] = id;
                return View(model);
            }

            var result = await _affiliateService.UpdateStudentService(model);
            if (result.success)
            {
                TempData["AffiliateMessage"] = result.Message;
                return RedirectToAction(nameof(Affiliates));
            }
            ModelState.AddModelError(string.Empty, result.Message);
            ViewData["AffiliateId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAffiliate(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction(nameof(Affiliates));
            var result = await _affiliateService.DeleteAffiliateByIdService(id);
            TempData["AffiliateMessage"] = result.Message;
            return RedirectToAction(nameof(Affiliates));
        }

        /// <summary>
        /// Saves file to wwwroot/uploads/media/ and records it in Media table; updates Project/BuildingDesign URL when purpose is Hero, Brochure, Image, or FloorPlan.
        /// </summary>
        private async Task<string?> SaveMediaFileAsync(IFormFile file, string purpose, string ownerType, string ownerId)
        {
            var isImage = string.Equals(purpose, "Hero", StringComparison.OrdinalIgnoreCase) || string.Equals(purpose, "Image", StringComparison.OrdinalIgnoreCase);
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedPdfExtension = ".pdf";
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (isImage && !allowedImageExtensions.Contains(ext))
                return null;
            if (!isImage && ext != allowedPdfExtension)
                return null;
            if (file.Length > 15 * 1024 * 1024) // 15MB max
                return null;
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "media");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);
            var storedPath = $"/uploads/media/{fileName}";
            await _mediaService.SaveMediaAsync(storedPath, file.FileName, file.ContentType ?? "application/octet-stream", file.Length, purpose, ownerType, ownerId);
            return storedPath;
        }
    }
}

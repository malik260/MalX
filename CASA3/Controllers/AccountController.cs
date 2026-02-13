using Core.Model;
using Core.ViewModels;
using Logic;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Util;

namespace CASA3.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(SignInManager<AppUser> signInManager,UserManager<AppUser> userManager, ILogger<AccountController> logger, IAccountService accountService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _accountService = accountService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true && Util.GetCurrentUser().Id != null)
                return Redirect(returnUrl ?? "/Admin");

            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/Admin");
            return View(new LoginVM { ReturnUrl = returnUrl ?? "/Admin" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null, CancellationToken cancellationToken = default)
        {
            returnUrl ??= Url.Content("~/Admin");
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!,model.Password,model.RememberMe,lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var sessionUser = new { user.Id, user.UserName, user.Email, user.FirstName, user.LastName, UserRole = userRoles.FirstOrDefault() };
                var userJson = JsonConvert.SerializeObject(sessionUser);
                HttpContext.Session.SetString("currentuser", userJson);
                _logger.LogInformation("User {Email} logged in.", model.Email);
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return Redirect(returnUrl ?? "/Admin");

            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/Admin");
            return View(new RegisterVM { ReturnUrl = returnUrl ?? "/Admin" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Admin");
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                    return View(model);
                }

                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateRegistered = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.AdminRole);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var sessionUser = new { user.Id, user.UserName, user.Email, user.FirstName, user.LastName };
                    HttpContext.Session.SetString("currentuser", JsonConvert.SerializeObject(sessionUser));
                    _logger.LogInformation("User {Email} registered and signed in.", model.Email);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
            
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("currentuser");
            _logger.LogInformation("User logged out.");
            return Redirect(returnUrl ?? Url.Content("~/"));
        }

        [AllowAnonymous]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
                ViewData["StatusCode"] = statusCode.Value;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> ChangePassword(string model)
        {
            var response = await _accountService.ChangePasswordService(model);
            return Json(response);
        }
    }
}

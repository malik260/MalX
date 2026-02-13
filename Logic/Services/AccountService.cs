using Core.DTOs;
using Core.Model;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Reflection;
using static Util;

namespace Logic.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILoggerManager _log;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountService(ILoggerManager log, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _log = log;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task CreateSuperAdmin()
        {
            try
            {
                var superAdminExists = await _userManager.FindByEmailAsync("SuperAdmin@casa3.com");
                if (superAdminExists == null)
                {
                    var user = new AppUser
                    {
                        UserName = "SuperAdmin@casa3.com",
                        Email = "SuperAdmin@casa3.com",
                        FirstName = "Super",
                        LastName = "Admin",
                        DateRegistered = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "cAsA@IiI");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, Constants.SuperAdminRole);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
            }
        }

        public async Task<HeplerResponseVM> ChangePasswordService(string details)
        {
            var response = new HeplerResponseVM();
            try
            {
                var model = JsonConvert.DeserializeObject<ChangePasswordDTO>(details);
                var userInSession = GetCurrentUser();
                if (userInSession == null)
                {
                    response.Message = "Error Occured"; return response;
                }
                var user = await _userManager.FindByIdAsync(userInSession.Id);
                if (user == null)
                {
                    response.Message = "Error Occured: Account Not Found"; return response;
                }
                var checkRes = await _signInManager.CheckPasswordSignInAsync(user, model.OldPassword, false).ConfigureAwait(false);
                if (!checkRes.Succeeded)
                {
                    response.Message = "Incorrect Old password submitted"; return response;
                }
                await _userManager.RemovePasswordAsync(user).ConfigureAwait(false);
                await _userManager.AddPasswordAsync(user, model.NewPassword).ConfigureAwait(false);

                response.success = true;
                response.Message = "Changed Successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }
    }
}

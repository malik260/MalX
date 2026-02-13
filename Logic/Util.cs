using Core.Model;
using Core.ViewModels;
using Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Security.Claims;
using System.Text;

namespace Logic
{
    public static class AppHttpContext
    {
        static IServiceProvider services = null;

        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        /// <summary>
        /// Provides static access to the current HttpContext
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return httpContextAccessor?.HttpContext;
            }
        }
        public class SessionTimeoutAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = AppHttpContext.Current;
                if (Current.Session.GetString("currentuser") == null)
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}

public class Util
{
    public static AppUserVM GetCurrentUser()
    {
        var loggInUser = new AppUserVM();
        var userString = AppHttpContext.Current?.Session?.GetString("currentuser");
        if (userString != null)
        {
            loggInUser = JsonConvert.DeserializeObject<AppUserVM>(userString);
            return loggInUser;
        }
        var userName = AppHttpContext.Current?.User?.Claims?.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
        loggInUser.UserName = userName;
        return loggInUser;
    }


    public static string GetEnumDescription(Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        {
            return attribute.Description;
        }
        return field?.ToString();
    }


    public static class Constants
    {
        public static string SuperAdminRole = "SuperAdmin";
        public static string AdminRole = "Admin";
        public static string StaffRole = "Staff";

        public static string DefaultLayout = "~/Views/Shared/_Layout.cshtml";
        public static string CMS_Layout = "~/Views/Shared/_AdminLayout.cshtml";
    }
}


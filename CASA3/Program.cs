using Core.DB;
using Core.Model;
using Logic;
using Logic.IServices;
using Logic.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<EFContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Casa3DbConnection")));
        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;

        }).AddEntityFrameworkStores<EFContext>();

        builder.Services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue;
            x.MultipartHeadersLengthLimit = int.MaxValue;
        });

        builder.Services.AddOptions();

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<ILoggerManager, LoggerManager>();
        builder.Services.AddScoped<IAffiliateService, AffiliateService>();
        builder.Services.AddScoped<IVendorService, VendorService>();
        builder.Services.AddScoped<INewsletterSubscriptionService, NewsletterSubscriptionService>();
        builder.Services.AddScoped<IContactUsService, ContactUsService>();
        builder.Services.AddScoped<IStaffService, StaffService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IMediaService, MediaService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<INewsLetterService, NewsLetterService>();
        builder.Services.AddScoped<IPartnerService, PartnerService>();
        builder.Services.AddScoped<IBlogService, BlogService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICarouselService, CarouselService>();
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromMinutes(43800);
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // Enable HSTS

            app.UseStatusCodePagesWithReExecute("/Account/Error/{0}");
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseCookiePolicy();
        app.UseRouting();
        app.UseAuthorization();
        app.UseSession();
        EnableSession(app);
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using (var scope = app.Services.CreateScope())
        {
            Console.WriteLine("CONNECTION = " +
            builder.Configuration.GetConnectionString("Casa3DbConnection"));

            var context = scope.ServiceProvider.GetRequiredService<EFContext>();
            context?.Database.Migrate();
            var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "SuperAdmin", "Admin", "Staff" };
            foreach (var role in roles)
            {
                if (!await roleManger.RoleExistsAsync(role))
                    await roleManger.CreateAsync(new IdentityRole(role));
            }
            var account_context = scope.ServiceProvider.GetRequiredService<IAccountService>();
            await account_context.CreateSuperAdmin();
        }
        app.Run();

        void EnableSession(IApplicationBuilder app)
        {
            //Enable Session.
            AppHttpContext.Services = app.ApplicationServices;
        }
    }
}
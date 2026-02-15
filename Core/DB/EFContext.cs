using Core.Model;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.DB
{
    public class EFContext : IdentityDbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
        }
        public DbSet<AppUser> ApplicationUsers { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<Affiliate> Affiliates { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<BuildingDesign> BuildingDesigns { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogView> BlogViews { get; set; }
    }
}

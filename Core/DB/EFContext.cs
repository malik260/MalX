using Core.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Core.DB
{
    public class EFContext : IdentityDbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
        }
        public DbSet<AppUser> ApplicationUsers { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
    }
}

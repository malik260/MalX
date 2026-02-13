using Core.Model;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Partner : BaseModelMain
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? ContactEmail { get; set; }

        [Phone]
        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [Required]
        public string LogoUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}

using Core.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class BlogView : BaseModel
    {
        [Required]
        public string BlogId { get; set; }

        [ForeignKey("BlogId")]
        public virtual Blog Blog { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(200)]
        public string? SystemName { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}


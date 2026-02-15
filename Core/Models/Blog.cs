using Core.Model;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Blog : BaseModelMain
    {
        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(500)]
        public string? Slug { get; set; }

        [StringLength(200)]
        public string? Category { get; set; }

        public DateTime? PublishedDate { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(1000)]
        public string? Excerpt { get; set; }

        [StringLength(500)]
        public string? CoverImageUrl { get; set; }

        [StringLength(200)]
        public string? Author { get; set; }

        public int Views { get; set; } = 0;

        public bool IsPublished { get; set; } = false;
    }
}


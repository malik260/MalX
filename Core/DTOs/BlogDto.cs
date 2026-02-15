using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class BlogDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Category { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string? Author { get; set; }
        public bool IsPublished { get; set; }
    }

    public class BlogUpdateDto : BlogDto
    {
        public string Id { get; set; } = string.Empty;
    }
}


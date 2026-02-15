namespace Core.ViewModels
{
    public class BlogVM
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Category { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Author { get; set; }
        public int Views { get; set; }
        public bool IsPublished { get; set; }
    }
}


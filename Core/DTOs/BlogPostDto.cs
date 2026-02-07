namespace Core.DTOs
{
    public class BlogPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Category { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string CoverImageUrl { get; set; }
        public string Author { get; set; }
        public int Views { get; set; }
    }
}

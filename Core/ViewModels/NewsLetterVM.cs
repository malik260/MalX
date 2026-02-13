using Microsoft.AspNetCore.Http;

namespace Core.ViewModels
{
    public class NewsLetterVM
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public string CoverImageUrl { get; set; }
        public string DocumentUrl { get; set; }
    }
}

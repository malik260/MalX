using Core.Enum;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class NewsletterDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public IFormFile? CoverImage { get; set; } //Nullable only during updates
        public IFormFile? Document { get; set; }//Nullable only during updates
    }

    public class NewsletterUpdateDto : NewsletterDto
    {
        public string Id { get; set; }
    }
}


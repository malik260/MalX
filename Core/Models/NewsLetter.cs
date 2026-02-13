using Core.Model;

namespace Core.Models
{
    public class NewsLetter : BaseModelMain
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public string CoverImageUrl { get; set; }
        public string DocumentUrl { get; set; }
    }
}

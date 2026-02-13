using Core.DTOs;

namespace Core.ViewModels
{
    public class HomePageVM
    {
        public List<BannerDto> Banner { get; set; } = new();
        public List<FeaturedProjectDto> FeaturedProjects { get; set; } = new();
        public List<FootprintYearDto> FootprintYears { get; set; } = new();
        public int SelectedFootprintYear { get; set; } = 2018;
        public List<NewsLetterVM> Newsletters { get; set; } = new();
        public List<TeamMemberDto> BoardOfDirectors { get; set; } = new();
        public List<TeamMemberDto> ManagementTeam { get; set; } = new();
        public List<BlogPostDto> BlogPosts { get; set; } = new();
        public List<BlogPostDto> RecentBlogPosts { get; set; } = new();
        public PaginationVM Pagination { get; set; } = new();
    }


    public class FeaturedProjectDto
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}

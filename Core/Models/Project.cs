using Core.Enum;

namespace Core.Model
{
    public class Project : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? HeroImageUrl { get; set; }
        public string? BrochurePdfUrl { get; set; }
        public int? Year { get; set; }
        public bool IsFeatured { get; set; }
        public ProjectCategory? Category { get; set; }
        public virtual ICollection<BuildingDesign> BuildingDesigns { get; set; } = new List<BuildingDesign>();
    }
}

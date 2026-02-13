using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class PartnerDto
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public IFormFile? Logo { get; set; } // Nullable only during updates
    }

    public class PartnerUpdateDto : PartnerDto
    {
        public string Id { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}


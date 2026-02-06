using Microsoft.AspNetCore.Identity;

namespace Core.Model
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePix { get; set; }
        public DateTime DateRegistered { get; set; }
        public bool IsDeactivated { get; set; }
        public DateTime? DateModified { get; set; }
    }   
}

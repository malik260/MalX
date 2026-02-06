using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    public class UserVerification
    {
        [Key]
        public Guid Token { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? DateVerified { get; set; }
        public DateTime? VerificationEmailSentDate { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }
    }
}

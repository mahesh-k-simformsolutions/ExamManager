using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManagementSystem.Data
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }

        public bool Verified { get; set; }


        // For dropdown
        [NotMapped]
        public bool NotVerified { get { return !Verified; } }

        public string VerifiedName { get { return Name + (Verified ? "" : " (Not Verified)"); } }

    }
}

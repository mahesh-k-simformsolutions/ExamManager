using Microsoft.AspNetCore.Identity;

namespace ExamManagementSystem.Data
{
    public class User : IdentityUser
    {
        public string? Email { get; set; }
        public string? Name { get; set; }

        public string? Role { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace ExamManagementSystem.Data
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
    }
}

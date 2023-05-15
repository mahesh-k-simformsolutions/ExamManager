using ExamManagementSystem.Data;
using ExamManagementSystem.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExamManagementSystem.Helpers
{
    public static class Helpers
    {
        public static string GenerateCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new();

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static async Task CreateAdmin(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string userName = "mnkantariya173@gmail.com";
            string password = "Gb3QC2YS@J7kg";
            string name = "Mahesh Kantariya";
            string roleName = EnumUserRole.Admin.ToString();

            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                _ = await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            User? user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    UserName = userName,
                    Email = userName
                };
                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    _ = await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal!.FindFirst(ClaimTypes.NameIdentifier.ToString())!.Value;
        }

        public static async Task<string> GetUserId(this AuthenticationStateProvider authenticationStateProvider)
        {
            var authstate = await authenticationStateProvider.GetAuthenticationStateAsync();
            return authstate.User.GetUserId();
        }

        public static bool IsAdminOrTeacher(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.IsInRole(EnumUserRole.Admin.ToString()) || claimsPrincipal.IsInRole(EnumUserRole.Teacher.ToString());
        }
    }

    public static class DateHelper
    {
        public static string ConvertToString(this DateTime date)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);
            DateTime yesterday = today.AddDays(-1);

            if (date.Date == today)
            {
                return "Today";
            }
            else
            {
                return date.Date == tomorrow ? "Tomorrow" : date.Date == yesterday ? "Yesterday" : date.ToString("D");
            }
        }
    }

}

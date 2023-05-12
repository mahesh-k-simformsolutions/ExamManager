using ExamManagementSystem.Enums;
using Radzen;
using System.Security.Claims;

namespace ExamManagementSystem.Helpers
{
    public static class Helpers
    {
        public static string GenerateCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal!.FindFirst(ClaimTypes.NameIdentifier.ToString())!.Value;
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
            else if (date.Date == tomorrow)
            {
                return "Tomorrow";
            }
            else if (date.Date == yesterday)
            {
                return "Yesterday";
            }
            else
            {
                return date.ToString("D");
            }
        }

    }
}

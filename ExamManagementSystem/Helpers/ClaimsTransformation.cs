using ExamManagementSystem.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ExamManagementSystem.Helpers
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly UserManager<User> _userManager;
        public ClaimsTransformation(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsPrincipal clone = principal.Clone();
            ClaimsIdentity newIdentity = (ClaimsIdentity)clone.Identity;
            string userId = principal.GetUserId();
            if (userId != null)
            {
                User user = null;// await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    System.Collections.Generic.IList<string> role = await _userManager.GetRolesAsync(user);
                    if (!principal.HasClaim(claim => claim.Type == newIdentity.RoleClaimType) && role.Count > 0)
                    {
                        newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, role[0]));
                    }
                }
            }
            return clone;
        }
    }

   
}

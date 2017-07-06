using System;
using System.Security.Claims;

namespace TeamService.WEB.Infrastructure
{
    public static class ClaimsPrincipalExtentions
    {
        private const string UserIdClaimName = "userId";

        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            try
            {
                var claim = principal.FindFirst(UserIdClaimName);

                if (claim == null)
                {
                    return null;
                }

                var guid = Guid.Parse(claim.Value);

                return guid;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
    }
}
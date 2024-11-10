using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername (this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;
        }


    }
}

//ClaimsPrincipal: Holds user details derived from the JWT. This extension method lets you retrieve the username from claims, which were added when the JWT was created.
//2. **Why Claims?**: JWT tokens include user info (claims) like username and email. This method simplifies getting the username directly from claims instead of passing parameters manually.
// 3. Check more claim type at: https://learn.microsoft.com/en-us/windows-server/identity/ad-fs/technical-reference/the-role-of-claims
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Auth
{
    public class DoNotAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            string[] usersSplit = SplitString(Users);
            if ((usersSplit.Length > 0) && usersSplit.Contains<string>(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            string[] rolesSplit = SplitString(Roles);
            if ((rolesSplit.Length > 0) && rolesSplit.Any<string>(new Func<string, bool>(user.IsInRole)))
            {
                return false;
            }
            return true;
        }

        internal static string[] SplitString(string original)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return new string[0];
            }
            return (from piece in original.Split(new[] { ',' })
                    let trimmed = piece.Trim()
                    where !string.IsNullOrEmpty(trimmed)
                    select trimmed).ToArray<string>();
        }
    }
}


using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using InService.Lib.Auth;

namespace InService.Web.Auth
{
    public static class AuthExtensions
    {
        public static bool HasRights(this IIdentity user, AccessRight right)
        {
            bool result = false;
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var Rights = userManager.FindById(user.GetUserId()).AccessRight;
            if (Rights.HasFlag(right)) result = true;
            return result;
        }
        public static bool HasRights(this IPrincipal user, AccessRight right)
        {
            bool result = false;
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var Rights = userManager.FindById(user.Identity.GetUserId()).AccessRight;
            if (Rights.HasFlag(right)) result = true;
            return result;
        }

        public static string FullName(this IIdentity user)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var Name = userManager.FindById(user.GetUserId()).Name;
            return Name;
        }
        public static string FullName(this IPrincipal user)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var Name = userManager.FindById(user.Identity.GetUserId()).Name;
            return Name;
        }
    }
}

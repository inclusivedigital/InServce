using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using InService.Lib.Auth;

namespace InService.Web.Auth
{
    public class InServiceIUserPrincipal : IPrincipal
    {
        public IIdentity Identity => UserIdentity;

        private InServiceIUserIdentity UserIdentity;

        public bool IsInRole(string role)
        {
            var uRole = (UserRole)Enum.Parse(typeof(UserRole), role);
            return UserIdentity.User.User.Role.HasFlag(uRole);
        }


        public InServiceIUserPrincipal(InServiceIUser user) => UserIdentity = new InServiceIUserIdentity(user);
    }
}
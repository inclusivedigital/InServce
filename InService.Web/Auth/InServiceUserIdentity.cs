using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace InService.Web.Auth
{
    public class InServiceIUserIdentity : ClaimsIdentity
    {
        public InServiceIUser User { get; }

        public override string AuthenticationType => DefaultAuthenticationTypes.ApplicationCookie;
        public override string Name => User.User.Email;
        public override bool IsAuthenticated => true;

        public InServiceIUserIdentity(InServiceIUser user)
        {
            User = user;
            AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            AddClaim(new Claim(ClaimTypes.Name, user.User.Name));
            if (!String.IsNullOrWhiteSpace(user.Email)) AddClaim(new Claim(ClaimTypes.Email, user.Email));
            AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", nameof(InServiceIUserIdentity)));
        }
    }
}
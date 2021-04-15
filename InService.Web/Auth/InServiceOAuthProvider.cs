using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace InService.Web.Auth
{
    public class InServiceAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public InServiceAuthProvider(string publicClientId)
        {
            _publicClientId = publicClientId ?? throw new ArgumentNullException("publicClientId");
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            InServiceIUser user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            if (user.User.Status == Lib.Auth.UserStatus.BLOCKED)
            {
                context.SetError("invalid_grant", "The user is blocked.");
                return;
            }
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager/*, OAuthDefaults.AuthenticationType*/);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager/*,CookieAuthenticationDefaults.AuthenticationType*/);

            AuthenticationProperties properties = CreateProperties(user);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");
                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(InServiceIUser user)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "loginID", user.UserName },
                { "userName", user.User.Name },
                { "userID", user.Id },
                { "roleID", user.User.RoleID.ToString() },
                { "rightsID", user.User.AccessRightID.HasValue?user.User.AccessRightID?.ToString():"0" },
            };
            return new AuthenticationProperties(data);
        }
    }
}
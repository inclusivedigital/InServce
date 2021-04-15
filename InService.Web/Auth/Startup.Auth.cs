﻿using System;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using InService.Web.Auth;

namespace InService.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, InServiceIUser>(
                      validateInterval: TimeSpan.FromHours(4),
                      regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),

                    OnApplyRedirect = (context) =>
                    {
                        if (!context.Request.Path.StartsWithSegments(new PathString("/api/")) && !context.Request.Headers.Keys.Contains("Authorization"))
                            context.Response.Redirect(context.RedirectUri);
                    },

                    OnException = (context) =>
                    {
                        context.Rethrow = false;
                        context.OwinContext.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        context.Response.Redirect("/Account/Login");
                    }
                }

            });
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "71511955362-hh045hgpl8t6t0j41pctfvniefrlriob.apps.googleusercontent.com",
            //    ClientSecret = "IJkhlErFBIQ2Da5pN4p8ixQP"
            //});


            //var facebookOptions = new FacebookAuthenticationOptions()
            //{
            //    AppId = "151446975479073",
            //    AppSecret = "563c75f822c97c1a23fb7b355af3a34a",
            //    BackchannelHttpHandler = new FacebookBackChannelHandler(),
            //    UserInformationEndpoint = "https://graph.facebook.com/v2.11/me?fields=id,email"
            //};
            //facebookOptions.Scope.Add("email");
            //app.UseFacebookAuthentication(facebookOptions);

            app.UseOAuthBearerTokens(new Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/Token"),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(90),
                AllowInsecureHttp = true,
                ApplicationCanDisplayErrors = true,
                Provider = new InServiceAuthProvider("mVC.Auth"),
            });
        }
    }
}
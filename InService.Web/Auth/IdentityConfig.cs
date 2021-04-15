using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using InService.Web.Auth;

namespace InService.Web
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<InServiceIUser>
    {
        public ApplicationUserManager(IUserStore<InServiceIUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new InServiceIUserStore());
            // Configure validation logic for user names
            manager.UserValidator = new UserValidator<InServiceIUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(25);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<InServiceIUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<InServiceIUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<InServiceIUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public override async Task<bool> CheckPasswordAsync(InServiceIUser user, string hash)
        {
            //var hash = VC.User.GetPasswordHash(user.UserName, password);
            if (user.PasswordHash == null) user.User.Hash = hash;
            if (user.PasswordHash == hash)
            {
                user.User.LastLoginDate = DateTime.Now;
                await Store.UpdateAsync(user);
            }
            return user.PasswordHash == null || user.PasswordHash == hash;
        }

        //public override Task<VC.User> FindAsync(string userName, string password)
        //{
        //    var user = Store.FindByNameAsync(userName).Result;
        //    if (user == null || user.PasswordHash != password) Task.FromResult<VC.User>(null);
        //   this. UpdateAsync(user);
        //    return Task.FromResult(user);
        //}
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<InServiceIUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        //public override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        //{
        //    var user = UserManager.FindByName(userName);
        //    if (user == null) return Task.FromResult(SignInStatus.Failure);
        //    return Task.FromResult(SignInStatus.Success);
        //}

        //public override Task<ClaimsIdentity> CreateUserIdentityAsync(VC.User user)
        //{
        //    return Task.FromResult<ClaimsIdentity>(new VC.serIdentity(user));
        //}

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}

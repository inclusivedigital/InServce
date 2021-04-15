using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using InService.Data;
using InService.Lib.Auth;

namespace InService.Web
{
    public class InServiceIUser : IUser<string>
    {
        public string Id => User.ID.ToString();

        public string UserName { get => User.LoginID; set => throw new NotImplementedException(); }

        public string Name
        {
            get => User.Name;
            set => User.Name = value;
        }

        public string PasswordHash { get => User.Hash; set => User.Hash = value; }

        public UserStatus Status { get => User.Status; set => User.Status = value; }

        public string PhoneNumber { get => User.Mobile; set => User.Mobile = value; }

        public AccessRight AccessRight { get => User.AccessRight; set => User.AccessRight = value; }

        public string Email { get => User.Email; set => User.Email = value; }

        public User User { get; }

        public InServiceIUser(User user) => User = user;

        public InServiceIUser() { }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<InServiceIUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        internal static string GetPasswordHash(string LoginID, string Password)
        {
            var SHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(LoginID.ToLower()));
            return Convert.ToBase64String(SHA1.ComputeHash(Encoding.UTF8.GetBytes(Password)));
        }
    }
}
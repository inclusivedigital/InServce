using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InService.Lib.Auth;

namespace InService.App.Auth
{
    class User
    {
        public int ID { get; set; }
        public string LoginID { get; set; }
        public string Name { get; set; }
        public string Hash { private get; set; }
        public DateTime LastServerLogin { get; private set; }
        public string AccessToken { get; private set; }
        public DateTime TokenExpiryDate { get; private set; }
        public string TokenType { get; private set; }

        private int RoleID;
        public UserRole Role => (UserRole)RoleID;

        private int AccessRightsID;
        public AccessRight AccessRights => (AccessRight)AccessRightsID;

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken) && TokenExpiryDate > DateTime.Now;

        public User(ISharedPreferences Prefs)
        {
            ID = Prefs.GetInt($"Usr_{nameof(ID)}", 0);
            RoleID = Prefs.GetInt($"Usr_{nameof(RoleID)}", 0);
            AccessRightsID = Prefs.GetInt($"Usr_{nameof(AccessRightsID)}", 0);
            LoginID = Prefs.GetString($"Usr_{nameof(LoginID)}", null);
            Hash = Prefs.GetString($"Usr_{nameof(Hash)}", null);
            Name = Prefs.GetString($"Usr_{nameof(Name)}", null);
            AccessToken = Prefs.GetString($"Usr_{nameof(AccessToken)}", null);
            TokenType = Prefs.GetString($"Usr_{nameof(TokenType)}", null);
            TokenExpiryDate = new DateTime(Prefs.GetLong($"Usr_{nameof(TokenExpiryDate)}", DateTime.Now.Ticks));
            LastServerLogin = new DateTime(Prefs.GetLong($"Usr_{nameof(LastServerLogin)}", DateTime.Now.Ticks));
        }

        public bool ValidHash(string hash) => Hash != null && hash == Hash;

        public User(dynamic JSON) => Sync(JSON);

        internal static string GetPasswordHash(string LoginID, string Password)
        {
            var SHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(LoginID.Trim().ToLower()));
            return Convert.ToBase64String(SHA1.ComputeHash(Encoding.UTF8.GetBytes(Password)));
        }

        public void Sync(ISharedPreferencesEditor Editor)
        {
            Editor.PutInt($"Usr_{nameof(ID)}", ID);
            Editor.PutInt($"Usr_{nameof(RoleID)}", RoleID);
            Editor.PutInt($"Usr_{nameof(AccessRightsID)}", AccessRightsID);
            Editor.PutString($"Usr_{nameof(LoginID)}", LoginID);
            Editor.PutString($"Usr_{nameof(Hash)}", Hash);
            Editor.PutString($"Usr_{nameof(Name)}", Name);
            Editor.PutString($"Usr_{nameof(TokenType)}", TokenType);
            Editor.PutString($"Usr_{nameof(AccessToken)}", AccessToken);
            Editor.PutLong($"Usr_{nameof(TokenExpiryDate)}", TokenExpiryDate.Ticks);
            Editor.PutLong($"Usr_{nameof(LastServerLogin)}", DateTime.Now.Ticks);
        }

        public void Sync(dynamic JSON)
        {
            ID = JSON.userID;
            RoleID = JSON.roleID;
            AccessRightsID = JSON.rightsID;
            LoginID = JSON.loginID;
            Name = JSON.userName;
            TokenType = JSON.token_type;
            AccessToken = JSON.access_token;
            int token_duration = JSON.expires_in;
            TokenExpiryDate = DateTime.Now.AddSeconds(token_duration - 60);
        }

        internal void Logout()
        {
            AccessToken = null;
            SessionManager.SaveSettings();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using InService.App.Auth;
using InService.App.Data;
using InService.App.UI;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace InService.App.Auth
{
    [Activity(Label = "Sign in..", Theme = "@style/AppTheme.Material")]
    public class LoginActivity : Activity
    {
        ProgressBar ProgressBar { get; set; }
        public TextInputEditText UserNameText { get; private set; }
        public TextInputEditText PasswordText { get; private set; }
        public Button LoginBtn { get; private set; }
        DataSync DataSync = new DataSync();
        TextInputLayout TILUsername, TILPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //ActionBar.Elevation = 0;
            SetContentView(Resource.Layout.auth_login);
            TILPassword = FindViewById<TextInputLayout>(Resource.Id.til_password);
            TILUsername = FindViewById<TextInputLayout>(Resource.Id.til_username);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.loginProgressBar);
            UserNameText = FindViewById<TextInputEditText>(Resource.Id.textUsername);
            PasswordText = FindViewById<TextInputEditText>(Resource.Id.textPassword);
            UserNameText.TextChanged += (o, e) => TILUsername.Error = null;
            PasswordText.TextChanged += (o, e) => TILPassword.Error = null;
            LoginBtn = FindViewById<Button>(Resource.Id.btnLogin);
            LoginBtn.Click += (o, e) => Login();
            FindViewById<CheckBox>(Resource.Id.checkboxShowPassword).Click += (o, e) =>
            {
                PasswordText.InputType = Android.Text.InputTypes.ClassText | (((CheckBox)o).Checked ? Android.Text.InputTypes.TextVariationVisiblePassword : Android.Text.InputTypes.TextVariationPassword);
            };
            if (!string.IsNullOrWhiteSpace(SessionManager.User.LoginID)) UserNameText.Text = SessionManager.User.LoginID;
            var username = Intent.GetStringExtra("Username");
            if (!string.IsNullOrWhiteSpace(username)) UserNameText.Text = username;
            DataSync.DataSyncCompleted += (o, e) =>
            {
                ProgressBar.Visibility = ViewStates.Gone;
            };
            DataSync.ProgressChanged += (o, e) => ProgressBar.Progress = e.Progress;
            DataSync.DataSyncFailed += (o, e) =>
            {
                MessageBox.Show(this, e.Message, "Sync failed");
                ProgressBar.Visibility = ViewStates.Gone;
            };
        }

        async void Login()
        {
            if (!Validate()) return;
            ProgressBar.Visibility = ViewStates.Visible;
            LoginBtn.Enabled = false;
            var pwdHash = User.GetPasswordHash(UserNameText.Text.Trim().ToLower(), PasswordText.Text);
            var client = SessionManager.GetHttpClient();
            var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username",UserNameText.Text.Trim().ToLower()),
                new KeyValuePair<string, string>("password", pwdHash ),
            });
            try
            {
                var response = await client.PostAsync($"{SessionManager.ServerAddress}/Token", content);
                var responseMsg = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseMsg))
                {
                    var json = JsonConvert.DeserializeObject<dynamic>(responseMsg);
                    if (response.IsSuccessStatusCode)
                    {
                        SessionManager.User.Sync(json);
                        SessionManager.SaveSettings();
                        SessionManager.User.Hash = pwdHash;
                        // SyncDataAsync();
                        if (SessionManager.IsFirstRun)
                        {
                            StartActivity(typeof(AboutUsActivity));
                            Finish();
                            OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                        }
                        else
                        {
                            StartActivity(typeof(MainActivity));
                            Finish();
                            OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                        }
                        //DataSync.ProgressChanged += (o, e) =>
                        //{
                        //    ProgressBar.Visibility = ViewStates.Visible;
                        //};
                        //DataSync.DataSyncCompleted += (o, e) =>
                        //{
                        //    ProgressBar.Visibility = ViewStates.Gone;
                        //    Finish();
                        //    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                        //};
                        //DataSync.DataSyncFailed += (o, e) =>
                        //{
                        //    MessageBox.Show(this, e.Message, "Sync failed");
                        //    ProgressBar.Visibility = ViewStates.Gone;
                        //};
                    }
                    else MessageBox.Show(this, "Login Error", (string)json.error_description);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "Login Error", e.Message);
            }
            LoginBtn.Enabled = true;
            ProgressBar.Visibility = ViewStates.Gone;
            this.HideSoftKeyboard();
        }

        bool Validate()
        {
            if (String.IsNullOrEmpty(UserNameText.Text))
            {
                TILUsername.Error = "User name is required!";
                return false;
            }
            if (PasswordText.Text.Length == 0)
            {
                TILPassword.Error = "Password is required!";
                return false;
            }
            return true;
        }

        //public override void OnBackPressed()
        //{
        //    MoveTaskToBack(true);
        //    Finish();
        //    OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
        //}
        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnBackPressed();
        }

        private async System.Threading.Tasks.Task SyncDataAsync()
        {
            if (!DataSync.IsBusy)
            {
                //  ProgressBar.Max = DataSync.ProgressMax;
                await DataSync.Download();
            }
            // IconsDataSync IconsDataSync = new IconsDataSync(this);
            // await IconsDataSync.Download(2);
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Auth
{
    [Activity(Label = "Login Options")]
    public class LoginOptionsActivity : AppCompatActivity
    {
        Button RegisterBtn, LoginBtn, ContinueBtn, AboutAppBtn, LanguageBtn;
        long lastPress;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.auth_options);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            RegisterBtn = FindViewById<Button>(Resource.Id.register);
            LoginBtn = FindViewById<Button>(Resource.Id.login);
            ContinueBtn = FindViewById<Button>(Resource.Id.guestuser);
            AboutAppBtn = FindViewById<Button>(Resource.Id.aboutapp);
            LanguageBtn = FindViewById<Button>(Resource.Id.language);
            InitHandlers();
        }
        void InitHandlers()
        {
            RegisterBtn.Click += (o, e) =>
            {
                StartActivity(typeof(RegistrationOptionsActivity));
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            };
            LoginBtn.Click += (o, e) =>
            {
                StartActivity(typeof(LoginActivity));
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            };
            ContinueBtn.Click += (o, e) => { };
            AboutAppBtn.Click += (o, e) =>
            {
                StartActivityForResult(typeof(AboutUsActivity), 0);
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            };
            LanguageBtn.Click += (o, e) =>
            {
                StartActivityForResult(typeof(LanguageOptionsActivity), 0);
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            };
        }
        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

        }

        public override void OnBackPressed()
        {
            long currentTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            if (currentTime - lastPress > 5000)
            {
                Toast.MakeText(this, "Press again to exit", ToastLength.Long).Show();
                lastPress = currentTime;
            }
            else
            {
                FinishAffinity();
            }
            //  base.OnBackPressed();
        }
    }
}
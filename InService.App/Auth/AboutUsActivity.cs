using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using InService.App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Auth
{
    [Activity(Label = "About Us")]
    public class AboutUsActivity : AppCompatActivity
    {
        Article CurArticle;
        WebView WebView;
        ImageButton BtnAttempt;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.about_us);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            WebView = FindViewById<WebView>(Resource.Id.webView1);
            WebView.Settings.JavaScriptEnabled = true;

            BtnAttempt = FindViewById<ImageButton>(Resource.Id.btn_floating_action);

        }
        protected override void OnResume()
        {
            base.OnResume();
            if (SessionManager.User.IsAuthenticated) SessionManager.IsFirstRun = false;
            if (CurArticle == null) LoadAboutUs();
        }


        void LoadAboutUs()
        {
            CurArticle = new Article
            {
                CreatorID = 1,
                CreationDate = DateTime.UtcNow,
                Name = "About In-service training app",
                FlagsID = 1,
                IsDefault = true,
                Description = SessionManager.AboutUs,
            };

            Title = $"{CurArticle.Name}";
            var HTML = CurArticle.Description;
            WebView.LoadData(HTML, "text/html", Encoding.UTF8.EncodingName);

            BtnAttempt.Click += (o, e) =>
            {
                if (SessionManager.User.IsAuthenticated)
                {
                    StartActivity(typeof(MainActivity));
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                else
                {
                    SetResult(Result.Ok);
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
            };
        }


        public override void OnBackPressed()
        {
            if (SessionManager.User.IsAuthenticated)
            {
                StartActivity(typeof(MainActivity));
                Finish();
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            }
            else
            {
                SetResult(Result.Canceled);
                Finish();
                OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
                base.OnBackPressed();
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if (SessionManager.User.IsAuthenticated)
                    {
                        StartActivity(typeof(MainActivity));
                        Finish();
                        OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                    }
                    else
                    {
                        SetResult(Result.Canceled);
                        Finish();
                        OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
                    }
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
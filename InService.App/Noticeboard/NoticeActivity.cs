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

namespace InService.App.Noticeboard
{
    [Activity(Label = "Notice")]
    public class NoticeActivity : AppCompatActivity
    {
        TextView TextHeading;
        WebView WebView;
        Guid NoticeID;
        Notice CurNotice;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Elevation = 0;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.notice);
            
            WebView = FindViewById<WebView>(Resource.Id.webView1);
            WebView.Settings.JavaScriptEnabled = true;
            TextHeading = FindViewById<TextView>(Resource.Id.text_heading);
        }
        protected override void OnResume()
        {
            base.OnResume();
            LoadNotice();
        }
        async void LoadNotice()
        {
            var uid = Intent.GetStringExtra("NoticeID");
            NoticeID = Guid.Parse(uid);
            if (CurNotice == null) CurNotice = await Notice.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == NoticeID);
            TextHeading.Text = CurNotice.Heading;
            var HTML = CurNotice.Description;
            WebView.LoadData(HTML, "text/html", Encoding.UTF8.EncodingName);
        }
        public override void OnBackPressed()
        {
            StartActivity(typeof(NoticeboardsActivity));
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnBackPressed();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    StartActivity(typeof(NoticeboardsActivity));
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
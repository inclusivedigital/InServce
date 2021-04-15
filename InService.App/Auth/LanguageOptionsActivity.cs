using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace InService.App.Auth
{
    [Activity(Label = "Select language")]
    public class LanguageOptionsActivity : AppCompatActivity
    {
        RadioButton BtnEnglish, BtnShona, BtnNdebele;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Context context = this;
            Android.Content.Res.Resources res = context.Resources;
            SessionManager.SetLocale(res);

            SetContentView(Resource.Layout.language_options);
            BtnEnglish = FindViewById<RadioButton>(Resource.Id.ChkEnglish);
            BtnShona = FindViewById<RadioButton>(Resource.Id.ChkShona);
            BtnNdebele = FindViewById<RadioButton>(Resource.Id.ChkNdebele);

            Title = res.GetString(Resource.String.select_language);
            FindViewById<ImageButton>(Resource.Id.btn_floating_action).Click += (o, ev) =>
            {
                if (BtnEnglish.Checked == true)
                {
                    SessionManager.LanguageCodevalue = "en";  
                }
                if (BtnShona.Checked == true)
                {
                    SessionManager.LanguageCodevalue = "sh";
                }
                if (BtnNdebele.Checked == true)
                {
                    SessionManager.LanguageCodevalue = "nd";
                }
                SetResult(Result.Ok);
                Finish();
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnBackPressed();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Canceled);
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
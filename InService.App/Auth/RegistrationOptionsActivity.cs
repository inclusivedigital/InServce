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
    [Activity(Label = "Registration options")]
    public class RegistrationOptionsActivity : AppCompatActivity
    {
        RadioButton BtnFarmer, BtnExtensionOfficer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.registration_options);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            BtnFarmer = FindViewById<RadioButton>(Resource.Id.ChkFarmer);
            BtnExtensionOfficer = FindViewById<RadioButton>(Resource.Id.ChkOfficer);
            FindViewById<Button>(Resource.Id.btnLogin).Click += (o, ev) =>
            {
                if (BtnFarmer.Checked == true)
                {
                    StartActivity(typeof(RegisterFarmerActivity));
                }
                if (BtnExtensionOfficer.Checked == true)
                {
                    StartActivity(typeof(RegisterExtensionOfficerActivity));
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
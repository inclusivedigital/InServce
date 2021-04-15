using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InService.App.Auth;
using InService.App.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.AuthTemp
{
    [Activity(Label = "Enter Temporary Pin")]
    public class EnterTemporaryPinActivity : AppCompatActivity
    {
        TextInputLayout TILPIN;
        EditText TextPIN;
        Android.Content.Res.Resources res;
        string CurPIN;
        ImageButton FAB;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            base.OnCreate(savedInstanceState);
            Context context = this;
            res = context.Resources;
            SessionManager.SetLocale(res);
            SetContentView(Resource.Layout.enter_secret_pin);

            TILPIN = FindViewById<TextInputLayout>(Resource.Id.til_pin);

            TextPIN = FindViewById<EditText>(Resource.Id.text_pin);

            FAB = FindViewById<ImageButton>(Resource.Id.btn_floating_action);

            InitHandlers();
            Title = "Enter PIN";
        }
        void InitHandlers()
        {
            TextPIN.TextChanged += (o, e) => TILPIN.Error = null;
            FAB.Click += (o, e) => Submit();
        }

        void Submit()
        {
            CurPIN = TextPIN.Text;
            if (!Validate()) return;
            if (CurPIN == SessionManager.SecretLock)
            {
                SessionManager.IsLocked = false;
                SetResult(Result.Ok);
                Finish();
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            }
            else MessageBox.Show(this, "Error", "Invalid PIN. Try again");
        }

        bool Validate()
        {
            var error = false;
            if (string.IsNullOrWhiteSpace(CurPIN))
            {
                TILPIN.Error = "Invalid PIN";
                error = true;
            }

            return !error;
        }
    }
}
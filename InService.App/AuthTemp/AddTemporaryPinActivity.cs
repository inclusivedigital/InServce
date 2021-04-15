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
    [Activity(Label = "Add Temporary Pin")]
    public class AddTemporaryPinActivity : AppCompatActivity
    {
        Spinner SpinnerHint;
        TextInputLayout TILPIN, TILCPIN, TILHintResponse;
        EditText TextPIN, TextCPIN, TextHintResponse;
        Android.Content.Res.Resources res;
        int CurHintID = -1;
        string CurPIN, HintResponse;

        ImageButton FAB;
        List<string> Hints;
        CheckBox CheckAgree;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            base.OnCreate(savedInstanceState);
            Context context = this;
            res = context.Resources;
            SessionManager.SetLocale(res);
            SetContentView(Resource.Layout.add_secret_lock);
            SpinnerHint = FindViewById<Spinner>(Resource.Id.spinner_hint);

            TILPIN = FindViewById<TextInputLayout>(Resource.Id.til_pin);
            TILCPIN = FindViewById<TextInputLayout>(Resource.Id.til_cpin);
            TILHintResponse = FindViewById<TextInputLayout>(Resource.Id.til_hint_response);

            TextPIN = FindViewById<EditText>(Resource.Id.text_pin);
            TextCPIN = FindViewById<EditText>(Resource.Id.text_cpin);
            TextHintResponse = FindViewById<EditText>(Resource.Id.text_hint_response);

            FAB = FindViewById<ImageButton>(Resource.Id.btn_floating_action);

            Hints = SessionManager.Hints();
            var hintsAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Hints);
            hintsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            SpinnerHint.Adapter = hintsAdapter;

            CheckAgree = FindViewById<CheckBox>(Resource.Id.checkbox_confirm);
            InitHandlers();
            Title = "Add PIN";
        }
        void InitHandlers()
        {
            TextPIN.TextChanged += (o, e) => TILPIN.Error = null;
            TextCPIN.TextChanged += (o, e) => TILCPIN.Error = null;
            TextHintResponse.TextChanged += (o, e) => TILHintResponse.Error = null;
            FAB.Click += (o, e) => Submit();
            SpinnerHint.ItemSelected += (o, e) =>
            {
                CurHintID = e.Position;
            };
        }

        void Submit()
        {
            CurPIN = TextPIN.Text;
            HintResponse = TextHintResponse.Text;
            if (!Validate()) return;
            SessionManager.SecretLock = TextPIN.Text;
            SessionManager.SecretHint = CurHintID;
            SessionManager.SecretAnswer = TextHintResponse.Text;
            SessionManager.IsLocked = true;
            SetResult(Result.Ok);
            Finish();
            OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
        }

        bool Validate()
        {
            var error = false;
            if (string.IsNullOrWhiteSpace(CurPIN))
            {
                TILPIN.Error = "Invalid PIN";
                error = true;
            }
            if (TextPIN.Text != TextCPIN.Text)
            {
                TILCPIN.Error = "PIN must match";
                error = true;
            }

            if (string.IsNullOrWhiteSpace(HintResponse))
            {
                TILHintResponse.Error = "Hint response is required";
                error = true;
            }
            if (CurHintID == -1)
            {
                MessageBox.Show(this, "Error", "Hint is required");
                error = true;
            }
            if (CheckAgree.Checked == false)
            {
                this.Show("Error","Verify and agree to continue");
                error = true;
            }
            return !error;
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InService.App.Data;
using InService.App.UI;
using InService.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InService.App.Auth
{
    [Activity(Label = "Farmer Registration", Theme = "@style/AppTheme.Material")]
    public class RegisterFarmerActivity : AppCompatActivity
    {
        TextInputLayout TILFirstName, TILSurname, TILDOB, TILFarmName, TILAddress, TILLocation, TILCity, TILMobile, TILEmail, TILNationalID, TILProvince, TILDistrict;
        TextInputEditText TextFirstName, TextLastName, TextDateOfBirth, TextFarmName, TextAddress, TextMobile, TextEmail, TextNationalID;
        Spinner SpinnerGender;
        AutoCompleteTextView TextLocation, TextCity, TextProvince, TextDistrict;
        Button BtnLogin;
        DateTime? DOB;
        Farmer CurMember = new Farmer();
        ProgressDialogueFragment progressDialogue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.registration_farmer);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            TILFirstName = FindViewById<TextInputLayout>(Resource.Id.til_fname);
            TILSurname = FindViewById<TextInputLayout>(Resource.Id.til_sname);
            TILDOB = FindViewById<TextInputLayout>(Resource.Id.til_dob);
            TILFarmName = FindViewById<TextInputLayout>(Resource.Id.til_farmname);
            TILAddress = FindViewById<TextInputLayout>(Resource.Id.til_address);
            TILLocation = FindViewById<TextInputLayout>(Resource.Id.til_location);
            TILCity = FindViewById<TextInputLayout>(Resource.Id.til_city);
            TILMobile = FindViewById<TextInputLayout>(Resource.Id.til_mobile);
            TILEmail = FindViewById<TextInputLayout>(Resource.Id.til_email);
            TILNationalID = FindViewById<TextInputLayout>(Resource.Id.til_nationalid);
            TILProvince = FindViewById<TextInputLayout>(Resource.Id.til_province);
            TILDistrict = FindViewById<TextInputLayout>(Resource.Id.til_district);

            TextFirstName = FindViewById<TextInputEditText>(Resource.Id.text_first_name);
            TextLastName = FindViewById<TextInputEditText>(Resource.Id.text_surname);
            SpinnerGender = FindViewById<Spinner>(Resource.Id.spinner_gender);
            TextDateOfBirth = FindViewById<TextInputEditText>(Resource.Id.text_dob);
            TextFarmName = FindViewById<TextInputEditText>(Resource.Id.text_farmname);
            TextAddress = FindViewById<TextInputEditText>(Resource.Id.text_address);
            TextLocation = FindViewById<AutoCompleteTextView>(Resource.Id.text_location);
            TextCity = FindViewById<AutoCompleteTextView>(Resource.Id.text_city);
            TextMobile = FindViewById<TextInputEditText>(Resource.Id.text_mobile);
            TextEmail = FindViewById<TextInputEditText>(Resource.Id.text_email);
            TextNationalID = FindViewById<TextInputEditText>(Resource.Id.text_nationalid);
            TextProvince = FindViewById<AutoCompleteTextView>(Resource.Id.text_province);
            TextDistrict = FindViewById<AutoCompleteTextView>(Resource.Id.text_district);

            BtnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            InitHandlers();
            BtnLogin.Click += (o, e) =>
            {
                Save();
            };
        }

        void ShowProgressDialogue(string status)
        {
            progressDialogue = new ProgressDialogueFragment(status);
            var trans = SupportFragmentManager.BeginTransaction();
            progressDialogue.Cancelable = false;
            progressDialogue.Show(trans, "progress");
        }
        void CloseProgressDialogue()
        {
            if (progressDialogue != null)
            {
                progressDialogue.Dismiss();
                progressDialogue = null;
            }
        }

        async Task RegistrationAsync()
        {
            ShowProgressDialogue("Registering...please wait!");
            try
            {
                var HttpClient = SessionManager.GetHttpClient();
                var json = JsonConvert.SerializeObject(CurMember);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = SessionManager.GetAPIURL("farmerRegistrations");
                var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content,
                };
                var response = await HttpClient.PostAsync(url, content);
                var responseMsg = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseMsg))
                {
                    var json1 = JsonConvert.DeserializeObject<dynamic>(responseMsg);
                    if (response.IsSuccessStatusCode)
                    {
                        CurMember.IsSynced = true;
                        Farmer.DB.Update(CurMember);
                        Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert = dialog.Create();
                        alert.SetTitle("Registration");
                        alert.SetCanceledOnTouchOutside(false);
                        alert.SetMessage("Registration was successful. You can now login. Your username is your mobile number. Use any PIN or password of choice.");
                        alert.SetIcon(Resource.Drawable.ic_account_key);
                        alert.SetButton("OK", (c, ev) =>
                        {
                            alert.Dismiss();
                            var intent = new Intent(this, typeof(LoginActivity));
                            intent.PutExtra("Username", CurMember.Mobile);
                            StartActivity(intent);
                            Finish();
                            OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                        });
                        alert.Show();
                    }
                    else
                    {
                        CloseProgressDialogue();
                        MessageBox.Show(this, "Regidtration Error", (string)json1.error_description);
                    }
                }
            }
            catch (Exception e)
            {
                CloseProgressDialogue();
                this.Show("Error", e.Message);
            }
        }



        void InitHandlers()
        {
            var genders = new List<string>();
            Enum.GetNames(typeof(Gender)).ToList().ForEach(c => genders.Add(c.ToEnumString()));
            SpinnerGender.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleDropDownItem1Line, genders);
            DatePickerFragment frag = new DatePickerFragment(TextDateOfBirth, SupportFragmentManager)
            {
                MinDate = DateTime.Now.AddYears(-100),
                MaxDate = DateTime.Now,
            };
            frag.DateSelected += (s, d) => { CurMember.DateOfBirth = d; DOB = d; };


            TextFirstName.TextChanged += (o, e) => TILFirstName.Error = null;
            TextLastName.TextChanged += (o, e) => TILSurname.Error = null;
            TextDateOfBirth.TextChanged += (o, e) => TILDOB.Error = null;
            TextFarmName.TextChanged += (o, e) => TILFarmName.Error = null;
            TextAddress.TextChanged += (o, e) => TILAddress.Error = null;
            TextLocation.TextChanged += (o, e) => TILLocation.Error = null;
            TextCity.TextChanged += (o, e) => TILCity.Error = null;
            TextMobile.TextChanged += (o, e) => TILMobile.Error = null;
            TextEmail.TextChanged += (o, e) => TILEmail.Error = null;
            TextNationalID.TextChanged += (o, e) => TILNationalID.Error = null;
            TextProvince.TextChanged += (o, e) => TILProvince.Error = null;
            TextDistrict.TextChanged += (o, e) => TILDistrict.Error = null;

            var cities = SessionManager.Cities();
            var locations = SessionManager.Locations();
            var provinces = SessionManager.Provinces();
            var districts = SessionManager.Districts();

            TextCity.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, cities);
            TextLocation.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, locations);
            TextProvince.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, provinces);
            TextDistrict.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, districts);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var farmer = Farmer.DB.Rows.FirstOrDefault();
            if (farmer != null)
            {
                CurMember = farmer;
                LoadMember();
                Farmer.DB.Truncate();
            }
        }

        void ExtractData()
        {
            CurMember.Address = TextAddress.Text.Trim();
            CurMember.Firstname = TextFirstName.Text.Trim();
            CurMember.NationalID = TextNationalID.Text.Trim();
            CurMember.Surname = TextLastName.Text.Trim();
            CurMember.Farmname = TextFarmName.Text.Trim();
            CurMember.Location = TextLocation.Text.Trim();
            CurMember.City = TextCity.Text.Trim();
            CurMember.Mobile = TextMobile.Text.Trim();
            CurMember.Email = TextEmail.Text.Trim();
            CurMember.Province = TextProvince.Text.Trim();
            CurMember.District = TextDistrict.Text.Trim();
            CurMember.GenderID = (SpinnerGender.SelectedItemPosition + 1);
        }

        async void Save()
        {
            ExtractData();
            if (Validate())
            {
                if (CurMember.ID == null || CurMember.ID == SessionManager.ID)
                {
                    CurMember.ID = Guid.NewGuid();
                    CurMember.IsSynced = false;
                    CurMember.RegistrationDate = DateTime.Now;
                    Farmer.DB.Insert(CurMember);
                }
                else Farmer.DB.Update(CurMember);
                if (!CurMember.IsSynced && Xamarin.Essentials.Connectivity.NetworkAccess.HasInternetAccess())
                {
                    await RegistrationAsync();
                }
                else
                {
                    MessageBox.Show(this, "Network error", "Your device is offline. Check your connectivity and submit again.");
                }
            }
        }


        bool Validate()
        {
            var error = false;
            if (String.IsNullOrWhiteSpace(CurMember.Firstname))
            {
                TILFirstName.Error = "Invalid first name";
                error = true;
            }
            if (String.IsNullOrWhiteSpace(CurMember.Surname))
            {
                TILSurname.Error = "Invalid surname";
                error = true;
            }
            if (DOB == null || (DateTime.Now - DOB.Value).TotalDays / 365 < 10)
            {
                TILDOB.Error = "Invalid date of birth";
                error = true;
            }
            if (String.IsNullOrWhiteSpace(CurMember.Address))
            {
                TILAddress.Error = "Invalid address";
                error = true;
            }
            if (String.IsNullOrWhiteSpace(CurMember.City))
            {
                TILCity.Error = "Invalid city";
                error = true;
            }
            if (String.IsNullOrWhiteSpace(CurMember.Farmname))
            {
                TILFarmName.Error = "Invalid farm name or plot";
                error = true;
            }
            if (String.IsNullOrWhiteSpace(CurMember.Mobile))
            {
                TILMobile.Error = "Invalid mobile number";
                error = true;
            }
            if (!String.IsNullOrEmpty(CurMember.Mobile) && !Regex.IsMatch(CurMember.Mobile, $@"({Regex.Escape("+")}?)\d{{8,14}}"))
            {
                TILMobile.Error = "Invalid mobile number";
                error = true;
            }
            if (!String.IsNullOrWhiteSpace(CurMember.Email))
            {
                try
                {
                    new System.Net.Mail.MailAddress(CurMember.Email);
                }
                catch
                {
                    TILEmail.Error = "Invalid email address";
                    error = true;
                }
            }
            return !error;
        }

        async void LoadMember()
        {
            CurMember = await Farmer.DB.RowsAsync.FirstOrDefaultAsync();
            TextAddress.Text = CurMember.Address;
            TextNationalID.Text = CurMember.NationalID;
            TextFirstName.Text = CurMember.Firstname;
            TextLastName.Text = CurMember.Surname;
            TextCity.Text = CurMember.City;
            TextLocation.Text = CurMember.Location;
            TextMobile.Text = CurMember.Mobile;
            TextEmail.Text = CurMember.Email;
            TextFarmName.Text = CurMember.Farmname;
            TextProvince.Text = CurMember.Province;
            TextDistrict.Text = CurMember.District;
            TextDateOfBirth.Text = CurMember.DateOfBirth?.ToString("dd-MMM-yyy");
            SpinnerGender.SetSelection(CurMember.GenderID);
            DOB = CurMember.DateOfBirth;
        }




    }
}
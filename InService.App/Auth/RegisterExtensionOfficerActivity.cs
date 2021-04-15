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
    [Activity(Label = "Extension Officer Registration")]
    public class RegisterExtensionOfficerActivity : AppCompatActivity
    {
        TextInputLayout TILFirstName, TILSurname, TILDOB, TILFarmName, TILMobile, TILEmail, TILNationalID, TILProvince, TILDistrict, TILECnumber;
        TextInputEditText TextFirstName, TextLastName, TextDateOfBirth, TextMobile, TextEmail, TextNationalID, TextECnumber;
        Spinner SpinnerGender;
        AutoCompleteTextView TextProvince, TextDistrict;
        Button BtnLogin;
        DateTime? DOB;
        ExtensionOfficer CurMember = new ExtensionOfficer();
        ProgressBar ProgressBar { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.registration_extensionofficer);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            TILFirstName = FindViewById<TextInputLayout>(Resource.Id.til_fname);
            TILSurname = FindViewById<TextInputLayout>(Resource.Id.til_sname);
            TILDOB = FindViewById<TextInputLayout>(Resource.Id.til_dob);
            TILFarmName = FindViewById<TextInputLayout>(Resource.Id.til_farmname);
            TILMobile = FindViewById<TextInputLayout>(Resource.Id.til_mobile);
            TILEmail = FindViewById<TextInputLayout>(Resource.Id.til_email);
            TILNationalID = FindViewById<TextInputLayout>(Resource.Id.til_nationalid);
            TILProvince = FindViewById<TextInputLayout>(Resource.Id.til_province);
            TILDistrict = FindViewById<TextInputLayout>(Resource.Id.til_district);
            TILECnumber = FindViewById<TextInputLayout>(Resource.Id.til_ecnumber);

            TextFirstName = FindViewById<TextInputEditText>(Resource.Id.text_first_name);
            TextLastName = FindViewById<TextInputEditText>(Resource.Id.text_surname);
            SpinnerGender = FindViewById<Spinner>(Resource.Id.spinner_gender);
            TextDateOfBirth = FindViewById<TextInputEditText>(Resource.Id.text_dob);
            TextMobile = FindViewById<TextInputEditText>(Resource.Id.text_mobile);
            TextEmail = FindViewById<TextInputEditText>(Resource.Id.text_email);
            TextNationalID = FindViewById<TextInputEditText>(Resource.Id.text_nationalid);
            TextProvince = FindViewById<AutoCompleteTextView>(Resource.Id.text_province);
            TextDistrict = FindViewById<AutoCompleteTextView>(Resource.Id.text_district);
            ProgressBar = FindViewById<ProgressBar>(Resource.Id.loginProgressBar);
            TextECnumber = FindViewById<TextInputEditText>(Resource.Id.text_ecnumber);

            BtnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            InitHandlers();
            BtnLogin.Click += (o, e) =>
            {
                Save();
            };
        }

        async Task RegistrationAsync()
        {
            ProgressBar.Visibility = ViewStates.Visible;
            BtnLogin.Enabled = false;
            try
            {
                var HttpClient = SessionManager.GetHttpClient();
                var json = JsonConvert.SerializeObject(CurMember);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = SessionManager.GetAPIURL("extensionOfficerRegistrations");
                var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content,
                };
                var response = await HttpClient.PostAsync(url, content);
                var responseMsg = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseMsg))
                {
                    var json1 = JsonConvert.DeserializeObject<ExtensionOfficer>(responseMsg);
                    if (response.IsSuccessStatusCode)
                    {
                        if (json1.ErrorCode.HasValue)
                        {
                            if (json1.ErrorCode == -1)
                            {
                                MessageBox.Show(this, "Record not found", json1.Firstname);
                            }
                            if (json1.ErrorCode == 1)
                            {
                                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                                Android.App.AlertDialog alert = dialog.Create();
                                alert.SetTitle("Record found");
                                alert.SetCanceledOnTouchOutside(false);
                                alert.SetMessage(json1.Firstname);
                                alert.SetIcon(Resource.Drawable.ic_account_key);
                                alert.SetButton("OK", (c, ev) =>
                                {
                                    alert.Dismiss();
                                    var intent = new Intent(this, typeof(LoginActivity));
                                    intent.PutExtra("Username", CurMember.ECNumber);
                                    StartActivity(intent);
                                    Finish();
                                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                                });
                                alert.Show();
                            }
                        }

                        else
                        {
                            CurMember.IsSynced = true;
                            ExtensionOfficer.DB.Update(CurMember);
                            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                            Android.App.AlertDialog alert = dialog.Create();
                            alert.SetTitle("Registration");
                            alert.SetCanceledOnTouchOutside(false);
                            alert.SetMessage("Registration was successful. You can now login. Your username is your EC number. Use any PIN or password of choice.");
                            alert.SetIcon(Resource.Drawable.ic_account_key);
                            alert.SetButton("OK", (c, ev) =>
                            {
                                alert.Dismiss();
                                var intent = new Intent(this, typeof(LoginActivity));
                                intent.PutExtra("Username", CurMember.ECNumber);
                                StartActivity(intent);
                                Finish();
                                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                            });
                            alert.Show();
                        }
                    }
                    else MessageBox.Show(this, "Regidtration Error", "Unknown error");
                }
            }
            catch (Exception e)
            {
                this.Show("Error", e.Message);
                BtnLogin.Enabled = true;
                ProgressBar.Visibility = ViewStates.Gone;
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
            TextMobile.TextChanged += (o, e) => TILMobile.Error = null;
            TextEmail.TextChanged += (o, e) => TILEmail.Error = null;
            TextNationalID.TextChanged += (o, e) => TILNationalID.Error = null;
            TextProvince.TextChanged += (o, e) => TILProvince.Error = null;
            TextDistrict.TextChanged += (o, e) => TILDistrict.Error = null;
            TextECnumber.TextChanged += (o, e) => TILECnumber.Error = null;

            var cities = SessionManager.Cities();
            var locations = SessionManager.Locations();
            var provinces = SessionManager.Provinces();
            var districts = SessionManager.Districts();

            TextProvince.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, provinces);
            TextDistrict.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, districts);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var oficer = ExtensionOfficer.DB.Rows.FirstOrDefault();
            if (oficer != null)
            {
                CurMember = oficer;
                LoadMember();
                ExtensionOfficer.DB.Truncate();
            }
        }

        void ExtractData()
        {
            CurMember.ECNumber = TextECnumber.Text.Trim();
            CurMember.Firstname = TextFirstName.Text.Trim();
            CurMember.NationalID = TextNationalID.Text.Trim();
            CurMember.Surname = TextLastName.Text.Trim();
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
                    ExtensionOfficer.DB.Insert(CurMember);
                }
                else ExtensionOfficer.DB.Update(CurMember);
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
            if (String.IsNullOrWhiteSpace(CurMember.ECNumber))
            {
                TILECnumber.Error = "Invalid EC number";
                error = true;
            }
            if (DOB == null || (DateTime.Now - DOB.Value).TotalDays / 365 < 10)
            {
                TILDOB.Error = "Invalid date of birth";
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
            CurMember = await ExtensionOfficer.DB.RowsAsync.FirstOrDefaultAsync();
            TextECnumber.Text = CurMember.ECNumber;
            TextNationalID.Text = CurMember.NationalID;
            TextFirstName.Text = CurMember.Firstname;
            TextLastName.Text = CurMember.Surname;
            TextMobile.Text = CurMember.Mobile;
            TextEmail.Text = CurMember.Email;
            TextProvince.Text = CurMember.Province;
            TextDistrict.Text = CurMember.District;
            TextDateOfBirth.Text = CurMember.DateOfBirth?.ToString("dd-MMM-yyy");
            SpinnerGender.SetSelection(CurMember.GenderID ?? 1);
            DOB = CurMember.DateOfBirth;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using InService.App.Data;
using System.IO;

namespace InService.App.Auth
{
    class SessionManager
    {
        static readonly ISharedPreferences SharedPrefs = Application.Context.GetSharedPreferences("InService", FileCreationMode.Private);

        public static User User { get; } = new User(SharedPrefs);
        public static Guid ID { get; } = Guid.Parse("00000000-0000-0000-0000-000000000000");
        public static string ArticleFiles { get; } = "Article_Downloads";

        public static string ServerAddress { get; private set; }

        public static string BluetoothPrinterAddress
        {
            get => SharedPrefs.GetString(nameof(BluetoothPrinterAddress), null);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutString(nameof(BluetoothPrinterAddress), value);
                Editor.Commit();
            }
        }

        public static decimal? VAT => 0m;
        public static string ReceiptHeader => "Items list";
        public static string ReceiptFooter => "Thank you for your business";

        public static bool AutoPrintReceipts
        {
            get => SharedPrefs.GetBoolean(nameof(AutoPrintReceipts), true);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(AutoPrintReceipts), value);
                Editor.Commit();
            }
        }

        public static DateTime LastDataSyncDate
        {
            get => new DateTime(SharedPrefs.GetLong(nameof(LastDataSyncDate), 0));
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutLong(nameof(LastDataSyncDate), value.Ticks);
                Editor.Commit();
            }
        }

        public static bool AutoSMSReceipts
        {
            get => SharedPrefs.GetBoolean(nameof(AutoSMSReceipts), false);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(AutoSMSReceipts), value);
                Editor.Commit();
            }
        }
        public static bool CanViewResults
        {
            get => SharedPrefs.GetBoolean(nameof(CanViewResults), false);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(CanViewResults), value);
                Editor.Commit();
            }
        }

        public static DateTime LastAppConfigSyncDate
        {
            get => new DateTime(SharedPrefs.GetLong(nameof(LastAppConfigSyncDate), 0));
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutLong(nameof(LastAppConfigSyncDate), value.Ticks);
                Editor.Commit();
            }
        }

        //  public static bool IsFirstRun => true;
        public static bool IsFirstRun
        {
            get => SharedPrefs.GetBoolean(nameof(IsFirstRun), true);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(IsFirstRun), value);
                Editor.Commit();
            }
        }

        public static bool SyncInvoices
        {
            get => SharedPrefs.GetBoolean(nameof(SyncInvoices), false);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(SyncInvoices), value);
                Editor.Commit();
            }
        }

        public static bool SyncAttachments
        {
            get => SharedPrefs.GetBoolean(nameof(SyncAttachments), false);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(SyncAttachments), value);
                Editor.Commit();
            }
        }
        public static bool SyncFiles
        {
            get => SharedPrefs.GetBoolean(nameof(SyncFiles), false);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(SyncFiles), value);
                Editor.Commit();
            }
        }

        static SessionManager()
        {
            LoadSettings();
        }

        public static void LoadSettings()
        {
            ServerAddress = SharedPrefs.GetString("ServerAddress", Application.Context.GetString(Resource.String.server_address));
        }

        public static string GetAPIURL(string API) => $"{ServerAddress}/api/{API}/";

        public static HttpClient GetHttpClient()
        {
            var client = new HttpClient
            {
                //MaxResponseContentBufferSize = 2560000
                //  MaxResponseContentBufferSize = 9999999
                MaxResponseContentBufferSize = int.MaxValue
            };
            if (User.IsAuthenticated)
            {
                client.DefaultRequestHeaders.Add("Authorization", $"{User.TokenType} {User.AccessToken}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            // client.Timeout = TimeSpan.FromMinutes(30);
            return client;
        }

        public static void SaveSettings()
        {
            var Editor = SharedPrefs.Edit();
            User.Sync(Editor);
            Editor.Commit();
        }

        public static string LanguageCodevalue
        {
            get => SharedPrefs.GetString(nameof(LanguageCodevalue), "en");
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutString(nameof(LanguageCodevalue), value);
                Editor.Commit();
                LoadSettings();
            }
        }

        public static void SetLocale(Android.Content.Res.Resources res)
        {
            DisplayMetrics Dm = res.DisplayMetrics;
            Android.Content.Res.Configuration conf = res.Configuration;
            if (LanguageCodevalue != null) conf.SetLocale(new Java.Util.Locale(LanguageCodevalue));
            else conf.SetLocale(new Java.Util.Locale("en"));    //Default Language
            res.UpdateConfiguration(conf, Dm);
        }

        public static bool IsLocked
        {
            get => SharedPrefs.GetBoolean(nameof(IsLocked), false);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutBoolean(nameof(IsLocked), value);
                Editor.Commit();
                LoadSettings();
            }
        }
        public static string SecretLock
        {
            get => SharedPrefs.GetString(nameof(SecretLock), "");
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutString(nameof(SecretLock), value);
                Editor.Commit();
                LoadSettings();
            }
        }
        public static int SecretHint
        {
            get => SharedPrefs.GetInt(nameof(SecretHint), 0);
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutInt(nameof(SecretHint), value);
                Editor.Commit();
                LoadSettings();
            }
        }
        public static string SecretAnswer
        {
            get => SharedPrefs.GetString(nameof(SecretAnswer), "");
            set
            {
                var Editor = SharedPrefs.Edit();
                Editor.PutString(nameof(SecretAnswer), value);
                Editor.Commit();
                LoadSettings();
            }
        }


        public static List<string> Hints()
        {
            return new List<string>
            {
                "My nick name",
                "My high school best frield",
                "My first cell phone",
                "My first phone number",
                "My first car",
                "My date of birth",
            };
        }
        public static List<string> Locations()
        {
            return new List<string>
            {
                "My nick name",
                "My high school best frield",
                "My first cell phone",
                "My first phone number",
                "My first car",
                "My date of birth",
            };
        }
        public static List<string> Cities()
        {
            return new List<string>
            {
                "My nick name",
                "My high school best frield",
                "My first cell phone",
                "My first phone number",
                "My first car",
                "My date of birth",
            };
        }
        public static List<string> Provinces()
        {
            return new List<string>
            {
                "My nick name",
                "My high school best frield",
                "My first cell phone",
                "My first phone number",
                "My first car",
                "My date of birth",
            };
        }
        public static List<string> Districts()
        {
            return new List<string>
            {
                "My nick name",
                "My high school best frield",
                "My first cell phone",
                "My first phone number",
                "My first car",
                "My date of birth",
            };
        }

        public static string AboutUs { get; } = "This <strong>In-service Training Application</strong> was developed by the <strong>Ministry of Agriculture (Agritex)</strong> in collaboration with Welthungerhilfe. " +
            "<br><br>The application was designed to equip the frontline extension workers throughout the nation with the latest and up to date information on field crops production, horticulture production, livestock production, agribusiness, marketing and cross-cutting issues such as gender and nutrition. " +
            "<br><br>This online platform has been necessitated, given the ever changing agricultural landscape with new technologies and techniques being introduced.It is against this background that training and refresher courses should be constantly reviewed and upgraded inline with the latest trends." +
            "<br><br>With the new focus, which requires that front-line extension workers train both crops and livestock production, the platform therefore provides new or refresh extensions knowledge to enhance capacities for them to effectively train farmers in Zimbabwe towards achieving the Government of Zimbabwe’s thrust of resuscitating the Agriculture sector." +
            "<br><br>This training application complements the Ministry of Lands Agriculture Water and Rural Resettlement physical in-service training offered by the department of AGRITEX." +
            "<br><br>Furthermore, the In-service training application has also been designed with smallholder farmers in mind.Farmers can also enrol on the same platform and take up the courses to sharpen agriculture knowledge on selected value chains of choice to improve their production, productivity and profitability of their farming enterprises." +
            "<br><br>In order to evaluate the level of comprehension, each course has an evaluation exercise and final examination." +
            "<br><br>These courses are offered to farmers on a cost recovery model as a small fee is levied.";



    }
}
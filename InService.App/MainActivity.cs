using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Interop;
using InService.App.Auth;
using InService.App.Data;
using InService.App.UI;
using InService.Lib;
using Newtonsoft.Json;
using Plugin.CurrentActivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using InService.App.Courses;
using AndroidX.Work;
using InService.App.Workers;
using Syncfusion.Licensing;
using InService.App.Downloads;

namespace InService.App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", Icon = "@mipmap/ic_launcher")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        RecyclerView GridViewMain;
        //List<MainMenuItem> MainMenuItems;
        List<MainMenuItemIcon> MainMenuItemIcons;
        ContentLoadingProgressBar LoadProgressBar;
        MainMenuIconAdapter Adapter;
        LinearLayout SyncLayout;
        int Action = 0;
        long lastPress;
        TextView ProgressView, EncryptView;
        Android.Content.Res.Resources res;
        bool IsSyncing = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            SyncfusionLicenseProvider.RegisterLicense("NDExOTM3QDMxMzgyZTM0MmUzMFZrM0k1K3BzaklSVVlIMGROOVVVK3dWYnJmUTJYUVNHUTNsMTZVY0d4aUE9");
            Context context = this;
            res = context.Resources;
            SessionManager.SetLocale(res);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            Android.Support.V7.App.ActionBarDrawerToggle toggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            GridViewMain = FindViewById<RecyclerView>(Resource.Id.gridView_main);
            GridViewMain.SetLayoutManager(new GridLayoutManager(this, 2));
            LoadProgressBar = FindViewById<ContentLoadingProgressBar>(Resource.Id.load_progress_bar);
            ProgressView = FindViewById<TextView>(Resource.Id.load_progress_bar_text);
            EncryptView = FindViewById<TextView>(Resource.Id.load_progress_bar_text_encrypt);
            SyncLayout = FindViewById<LinearLayout>(Resource.Id.sync_layout);
            FindViewById<ImageButton>(Resource.Id.btn_floating_action).Click += (o, e) =>
            {
                if (SessionManager.User.IsAuthenticated)
                {
                    SyncData();
                }
            };
            Title = res.GetString(Resource.String.app_title);



            // WorkRequest = PeriodicWorkRequest.Builder.From<DownloadsWorker>(TimeSpan.FromMinutes(20)).Build();
        }



        protected override async void OnResume()
        {
            base.OnResume();
            if (!SessionManager.User.IsAuthenticated)
            {
                Action = -1;
                StartActivity(typeof(LoginOptionsActivity));
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            }
            if (SessionManager.User.IsAuthenticated)
            {
                if (SessionManager.IsFirstRun)
                {
                    StartActivityForResult(typeof(AboutUsActivity), 0);
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                else if (SessionManager.IsLocked)
                {
                    StartActivityForResult(typeof(AuthTemp.EnterTemporaryPinActivity), 0);
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                else
                {
                    var branches = await Branch.DB.RowsAsync.ToListAsync();
                    if (branches.Count == 0)
                    {
                        SyncData();
                    }
                    PopulatMainMenuAsync();
                }

                if (SessionManager.SyncAttachments) SyncAttachments();
                if (SessionManager.SyncFiles) DownloadFiles();

            }
        }

        void PopulatMainMenuAsync()
        {
            var itemTint = Android.Graphics.Color.Green.ToArgb();
            var branches = Branch.DB.Rows.OrderBy(c => c.Name).ToList();

            MainMenuItemIcons = new List<MainMenuItemIcon>();
            if (branches.Count > 0)
            {
                foreach (var item in branches)
                {
                    if (MainMenuItemIcons.Any(c => c.ID == item.ID)) continue;
                    var iconItem = new MainMenuItemIcon { ID = item.ID, Title = item.Name, IsEnabled = true };
                    if (item.IconID.HasValue)
                    {
                        var attachment = Attachment.DB.Rows.FirstOrDefault(c => c.ID == item.IconID);
                        if (attachment != null) iconItem.Base64String = attachment.Data;
                    }
                    MainMenuItemIcons.Add(iconItem);
                }
            }
            if (!MainMenuItemIcons.Any(c => c.ID == 0))
                MainMenuItemIcons.Add(new MainMenuItemIcon { ID = 0, Title = "Notice board", IsEnabled = true });
            if (!MainMenuItemIcons.Any(c => c.ID == -1))
                MainMenuItemIcons.Add(new MainMenuItemIcon { ID = -1, Title = "Weather", IsEnabled = true });

            Adapter = new MainMenuIconAdapter(MainMenuItemIcons.ToArray());
            Adapter.ItemClick += (o, e) =>
            {
                var item = MainMenuItemIcons[e.Position];
                if (branches.Count > 0)
                {
                    foreach (var branch in branches)
                    {
                        if (item.ID == branch.ID)
                        {
                            var intent = new Intent(this, typeof(AllCoursesActivity));
                            intent.PutExtra("BranchID", branch.ID);
                            StartActivity(intent);
                            OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                        }
                    }
                }
                if (item.ID == 0)
                {
                    StartActivity(typeof(Noticeboard.NoticeboardsActivity));
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                if (item.ID == -1)
                {
                    StartActivity(typeof(Weather.MainWeatherActivity));
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                //else if (item.Label == "EXAMINATIONS")
                //{
                //    StartActivity(typeof(Examinations.ExaminationsActivity));
                //    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                //}
            };
            GridViewMain.SetAdapter(Adapter);
            Adapter.NotifyDataSetChanged();
        }


        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                long currentTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
                if (currentTime - lastPress > 5000)
                {
                    Toast.MakeText(this, "Press again to exit", ToastLength.Long).Show();
                    lastPress = currentTime;
                }
                else
                {
                    Action = int.MaxValue;
                    if (Xamarin.Essentials.Connectivity.NetworkAccess.HasInternetAccess()) SyncData();
                    else FinishAffinity();
                    // base.OnBackPressed();
                }
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
            }
            return base.OnOptionsItemSelected(item);
        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.nav_sign_out)
            {
                SignOut();
            }
            else if (id == Resource.Id.nav_exercises)
            {
                StartActivity(typeof(Exercises.MyExercisesActivity));
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            }
            else if (id == Resource.Id.nav_courses)
            {
                StartActivity(typeof(AllCoursesActivity));
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            }
            else if (id == Resource.Id.nav_weather)
            {
                StartActivity(typeof(Weather.MainWeatherActivity));
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            }
            else if (id == Resource.Id.nav_share)
            {
                StartActivity(typeof(Articles.ReaderActivity));
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            }
            else if (id == Resource.Id.nav_send)
            {
            }
            else if (id == Resource.Id.nav_change_language)
            {
            }
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        async Task LocationAsync()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var myPosition = await locator.GetPositionAsync();
            var location = new Cordinates
            {
                Latitude = myPosition.Latitude,
                Longitude = myPosition.Longitude
            };

            try
            {
                HttpClient HttpClient = SessionManager.GetHttpClient();
                var json = JsonConvert.SerializeObject(location);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = SessionManager.GetAPIURL("cordinates");
                var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content,
                };
                var response = await HttpClient.PostAsync(url, content);
                if (response != null && response.IsSuccessStatusCode)
                {
                }
            }
            catch (Exception e)
            {
                this.Show("Error", e.Message);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok) Recreate();
            base.OnActivityResult(requestCode, resultCode, data);
        }
        void DownloadFiles()
        {
            var download = new AndroidDownloader();
            var attachmemts = Attachment.DB.Rows.Where(c => c.IsDownloaded == false).ToList();
            if (attachmemts.Count > 0)
            {
                foreach (var item in attachmemts)
                {
                    download.DownloadFile($"{SessionManager.ServerAddress}/Attachments/Download/{item.ID}", item.Extension, SessionManager.ArticleFiles);
                    item.IsDownloaded = true;
                    Attachment.DB.Update(item);
                }
                download.OnFileDownloaded += OnFileDownloaded;
                SessionManager.SyncFiles = false;
            }
        }

        private void OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            if (e.FileSaved)
            {
                MessageBox.ShowToast(this, "File Saved Successfully");
            }
            else
            {
                MessageBox.ShowToast(this, "Error while saving the file");
            }
        }

        private async void SyncData()
        {
            var DataSync = new DataSync();
            IsSyncing = true;
            DataSync.DataSyncCompleted += (o, e) =>
            {
                LoadProgressBar.Hide();
                ProgressView.Visibility = ViewStates.Gone;
                EncryptView.Visibility = ViewStates.Gone;
                SyncLayout.Visibility = ViewStates.Gone;
                IsSyncing = false;
                PopulatMainMenuAsync();
            };
            DataSync.ProgressChanged += (o, e) =>
            {
                SyncLayout.Visibility = ViewStates.Visible;
                LoadProgressBar.Progress = e.Progress;
                ProgressView.Visibility = ViewStates.Visible;
                EncryptView.Visibility = ViewStates.Visible;
                IsSyncing = true;
                ProgressView.Text = e.Message;
            };
            DataSync.DataSyncFailed += (o, e) =>
            {
                MessageBox.Show(this, e.Message, "We could not sync");
                SyncLayout.Visibility = ViewStates.Gone;
                LoadProgressBar.Hide();
                ProgressView.Visibility = ViewStates.Gone;
                EncryptView.Visibility = ViewStates.Gone;
                IsSyncing = false;
                PopulatMainMenuAsync();
            };
            if (!DataSync.IsBusy)
            {
                SyncLayout.Visibility = ViewStates.Visible;
                LoadProgressBar.Show();
                ProgressView.Visibility = ViewStates.Visible;
                LoadProgressBar.Max = DataSync.ProgressMax;
                IsSyncing = true;
                await DataSync.Download();
                await LocationAsync();
            }
        }

        async void SyncAttachments()
        {
            if (IsSyncing || !Xamarin.Essentials.Connectivity.NetworkAccess.HasInternetAccess()) return;
            IsSyncing = true;
            var invSync = new AttachmentsDataSync();
            await invSync.SyncAttachments();
            IsSyncing = false;
        }

        void SignOut()
        {
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Sign out");
            alert.SetCanceledOnTouchOutside(false);
            alert.SetMessage("Are you sure you want to sign out");
            alert.SetIcon(Resource.Drawable.ic_account_key);
            alert.SetButton("Yes", (c, ev) =>
            {
                Action = 3;
                alert.Dismiss();
                // if (Xamarin.Essentials.Connectivity.NetworkAccess.HasInternetAccess()) SyncData();
                if (!IsSyncing) SessionManager.User.Logout();
                FinishAffinity();
                //  StartActivity(typeof(LoginOptionsActivity));
                // OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);

            });

            alert.SetButton2("Cancel", (c, ev) =>
            {
                alert.Dismiss();
            });
            alert.SetButton3("Lock", (c, ev) =>
            {
                if (!string.IsNullOrWhiteSpace(SessionManager.SecretLock))
                {
                    SessionManager.IsLocked = true;
                    FinishAffinity();
                }
                else
                {
                    StartActivityForResult(typeof(AuthTemp.AddTemporaryPinActivity), 0);
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
            });
            alert.Show();
        }

    }
}
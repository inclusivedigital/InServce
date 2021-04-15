using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using InService.App.Data;
using InService.App.Exercises;
using Android.Views.InputMethods;
using Syncfusion.SfPdfViewer.Android;
using InService.App.Auth;
using Android.Util;

namespace InService.App.Articles
{
    [Activity(Label = "Article")]
    public class ArticleActivity : AppCompatActivity
    {
        Article CurArticle;
        WebView WebView;
        ImageButton BtnAttempt;

        SfPdfViewer pdfViewer;
        //EditText pageNumberEntry;
        //TextView pageCountText;
        //ImageButton pageDownButton;
        //ImageButton pageUpButton;
        LinearLayout pdfMainView, webLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.article);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            WebView = FindViewById<WebView>(Resource.Id.webView1);
            WebView.Settings.JavaScriptEnabled = true;
            WebView.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);

            BtnAttempt = FindViewById<ImageButton>(Resource.Id.btn_floating_action);

            pdfViewer = FindViewById<SfPdfViewer>(Resource.Id.pdfviewercontrol);
         //   pageNumberEntry = FindViewById<EditText>(Resource.Id.pagenumberentry);
          //  pageCountText = FindViewById<TextView>(Resource.Id.pagecounttext);
           // pageDownButton = FindViewById<ImageButton>(Resource.Id.pagedownbutton);
           // pageUpButton = FindViewById<ImageButton>(Resource.Id.pageupbutton);
            pdfMainView = FindViewById<LinearLayout>(Resource.Id.parentview);
            webLayout = FindViewById<LinearLayout>(Resource.Id.webview);

            //Wireup events.





            InitHandlers();
        }

        void InitHandlers()
        {

        }

        protected override void OnResume()
        {
            base.OnResume();
            if (CurArticle == null) LoadArticle();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.banking, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        async void LoadArticle()
        {
            int ArticleID = Intent.GetIntExtra("ArticleID", 0);
            var paymentMethod = Intent.GetStringExtra("PaymentMethod");
            var currency = Intent.GetStringExtra("Currency");
            CurArticle = await Article.DB.RowsAsync.Where(c => c.ID == ArticleID).FirstOrDefaultAsync();

            Title = $"{CurArticle.Name}";
            await CurArticle.LoadCourse();
            await CurArticle.LoadValueChain();
            await CurArticle.LoadAttachment();
            if (CurArticle.AttachmentID.HasValue)
            {
                webLayout.Visibility = ViewStates.Gone;
                pdfMainView.Visibility = ViewStates.Visible;

             //   pageNumberEntry.KeyPress += PageNumberEntry_KeyPress;
              //  pageDownButton.Click += PageDownButton_Click;
               // pageUpButton.Click += PageUpButton_Click;
                pdfViewer.DocumentLoaded += PdfViewer_DocumentLoaded;
                pdfViewer.PageChanged += PdfViewer_PageChanged;
                //byte[] bytes = Convert.FromBase64String(CurArticle.Data);
                string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), SessionManager.ArticleFiles, $"{CurArticle.AttachmentID}{CurArticle.Attachment.Extension}");
                Stream PdfStream = new MemoryStream(File.ReadAllBytes(path));// new MemoryStream(bytes); // Assets.Open("sample.pdf");
                pdfViewer.LoadDocument(PdfStream);
            }
            else
            {
                webLayout.Visibility = ViewStates.Visible;
                pdfMainView.Visibility = ViewStates.Gone;
                var HTML = "";
                // if (CurArticle.AttachmentID.HasValue)
                // {
                //    var data = CurArticle.Data;

                // }
                //  else
                //  {
                HTML = CurArticle.Description;
                // }
                var attachments = CurArticle.Attachments;
                if (attachments.Any())
                {
                    foreach (var item in attachments)
                    {
                        var attachment = Attachment.DB.Rows.FirstOrDefault(c => c.ID == item.ID);
                        if (attachment != null)
                        {
                            //   string filename = $"{attachment.ID}{attachment.Extension}";
                            // HTML = HTML.Replace($"/Articles/Image?data={item.ID}", $"data:image/jpeg;base64,{attachment.Data}");
                            if (attachment.IsDownloaded)
                            {
                                var metrics = Resources.DisplayMetrics;
                                var width = metrics.WidthPixels;
                                string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), SessionManager.ArticleFiles, $"{attachment.ID}{attachment.Extension}");
                                HTML = HTML.Replace($"src=\"/Articles/Image?data={item.ID}\"", $"src=\"file://{path}\" style='width:{width}px;' ");
                            }
                        }
                    }
                }

                WebView.LoadDataWithBaseURL("", HTML, "text/html", Encoding.UTF8.EncodingName, "");
            }
            if (CurArticle.ModuleID.HasValue)
            {
                await CurArticle.LoadModule();
                var module = CurArticle.Module;
                await module.LoadExaminations();
                var examinatios = module.Examinations;
                BtnAttempt.Visibility = examinatios.Any() ? ViewStates.Visible : ViewStates.Gone;
            }

            BtnAttempt.Click += (o, e) =>
            {
                if ((bool)(CurArticle?.Module?.Examinations?.Any()))
                {
                    var examination = CurArticle.Module.Examinations.FirstOrDefault();
                    var intent = new Intent(this, typeof(GetReadyActivity));
                    intent.PutExtra("ExaminationID", examination.ID);
                    intent.PutExtra("ModuleID", CurArticle.Module.ID);
                    intent.PutExtra("ArticleID", CurArticle.ID);
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
            };

        }
        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnBackPressed();
        }


        private void PdfViewer_PageChanged(object sender, PageChangedEventArgs args)
        {
           // pageNumberEntry.Text = args.PageNumber.ToString();
        }

        private void PdfViewer_DocumentLoaded(object sender, System.EventArgs args)
        {
          //  pageNumberEntry.Text = pdfViewer.PageNumber.ToString();
          //  pageCountText.Text = pdfViewer.PageCount.ToString();
        }

        private void PageUpButton_Click(object sender, System.EventArgs e)
        {
            pdfViewer.GoToPreviousPage();
        }

        private void PageDownButton_Click(object sender, System.EventArgs e)
        {
            pdfViewer.GoToNextPage();
        }

        //private void PageNumberEntry_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        //{
        //    e.Handled = false;
        //    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
        //    {
        //        int pageNumber = 0;
        //        if (int.TryParse((pageNumberEntry.Text), out pageNumber))
        //        {
        //            if ((pageNumber > 0) && (pageNumber <= pdfViewer.PageCount))
        //                pdfViewer.GoToPage(pageNumber);
        //            else
        //            {
        //                DisplayAlertDialog();
        //                pageNumberEntry.Text = pdfViewer.PageNumber.ToString();
        //            }
        //        }
        //        pageNumberEntry.ClearFocus();
        //        InputMethodManager inputMethodManager = (InputMethodManager)pdfMainView.Context.GetSystemService(Context.InputMethodService);
        //        inputMethodManager.HideSoftInputFromWindow(pdfMainView.WindowToken, HideSoftInputFlags.None);
        //    }
        //}

        void DisplayAlertDialog()
        {
            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(pdfMainView.Context);
            alertDialog.SetTitle("Error");
            alertDialog.SetMessage("Please enter the valid page number");
            alertDialog.SetPositiveButton("OK", (senderAlert, args) => { });
            Android.App.Dialog dialog = alertDialog.Create();
            dialog.Show();
        }

    }
}
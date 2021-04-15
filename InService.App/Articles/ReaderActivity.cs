using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using PdfViewer;
using System.Collections.Generic;

using Android.Provider;
using StringBuilder = System.Text.StringBuilder;
using PdfViewer.Listener;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using PdfViewer.Scroll;
using PdfViewer.Util;
using PdfViewer.PDFium;
using Android.Util;
using Android.Content.PM;
using Uri = Android.Net.Uri;
using Android.Webkit;
using System;
using System.IO;

namespace InService.App.Articles
{
    [Activity(Label = "ReaderActivity")]
    public class ReaderActivity : AppCompatActivity
    {
        private string _documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        private WebView _webView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.pdfRenderer);
           // var stream = new StreamReader(Assets.Open("test.pbf"));
            using (StreamReader reader = new StreamReader(Assets.Open("test.pdf")))
            {
                using (var stream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(stream);

                }

            }
          //  _webView = FindViewById<WebView>(Resource.Id.webView1);
            //var settings = _webView.Settings;
            //settings.JavaScriptEnabled = true;
            //settings.AllowFileAccessFromFileURLs = true;
            //settings.AllowUniversalAccessFromFileURLs = true;
            //settings.BuiltInZoomControls = true;
            //  _webView.SetWebChromeClient(new WebChromeClient());

            //  DownloadPDFDocument();
          //  _webView.LoadUrl("file:///android_asset/pdfviewer/index.html?file=test.pdf");
        }

        protected override void OnResume()
        {
            base.OnResume();
          //  _webView.LoadUrl("javascript:window.location.reload( true )");
        }

        protected override void OnPause()
        {
            base.OnPause();
         //   _webView.ClearCache(true);
        }

    }
}
using System.IO;
using Android.App;
using Android.Net;
using Android.OS;
using Android.Webkit;
using File = Java.IO.File;

namespace XFPDFMerge.Droid.Activities
{
    [Activity(Label = "PdfWebViewActivity")]
    public class PdfWebViewActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var path = Intent.GetStringExtra("filePath");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            SetContentView(Resource.Layout.pdfInWebView);

            var pdfWebView = FindViewById<WebView>(Resource.Id.pdfWebView);
            if (pdfWebView == null)
            {
                return;
            }

            pdfWebView.Settings.AllowUniversalAccessFromFileURLs = true;

            using (var file = new File(Environment.ExternalStorageDirectory.AbsolutePath, Path.GetFileName(path)))
            {
                pdfWebView.LoadUrl($"file:///android_asset/pdfjs/web/viewer.html?file={Uri.FromFile(file)}");
            }
        }
    }
}
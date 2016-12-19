using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XFPDFMerge.CustomControls;
using XFPDFMerge.Droid.CustomRenderers;
using XFPDFMerge.Droid.Helpers;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

[assembly: ExportRenderer(typeof(PdfWebView), typeof(PdfWebViewRenderer))]
namespace XFPDFMerge.Droid.CustomRenderers
{
    public class PdfWebViewRenderer : WebViewRenderer
    {
        protected override async void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e?.OldElement != null || e?.NewElement == null)
            {
                return;
            }

            var pdfFile = (Element as PdfWebView)?.PdfFile;
            if (pdfFile == null)
            {
                return;
            }

            var filePath = await FileHelper.WriteToFileInStorage(pdfFile);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            Control.SetWebChromeClient(new FormsWebChromeClient());
            Control.Settings.AllowUniversalAccessFromFileURLs = Control.Settings.AllowFileAccess = Control.Settings.AllowFileAccessFromFileURLs = true;
            Control.Settings.JavaScriptEnabled = true;

            using (var file = new File(Environment.ExternalStorageDirectory.AbsolutePath, Path.GetFileName(filePath)))
            {
                Control.LoadUrl($"file:///android_asset/pdfjs/web/viewer.html?file={Uri.FromFile(file)}");
            }
        }
    }
}
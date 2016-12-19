using Xamarin.Forms;
using XFPDFMerge.Entities;

namespace XFPDFMerge.CustomControls
{
    public class PdfWebView : WebView
    {
        public static readonly BindableProperty PdfFileProperty = BindableProperty.Create(nameof(PdfFile), typeof(FileEntity), typeof(PdfWebView));

        public FileEntity PdfFile
        {
            get { return GetValue(PdfFileProperty) as FileEntity; }
            set { SetValue(PdfFileProperty, value); }
        }
    }
}
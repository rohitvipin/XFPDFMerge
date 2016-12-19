using Xamarin.Forms;
using XFPDFMerge.ViewModels;

namespace XFPDFMerge.Views
{
    public partial class PdfView : ContentPage
    {
        public PdfView(PdfViewModel pdfViewModel)
        {
            BindingContext = pdfViewModel;
            InitializeComponent();
        }
    }
}

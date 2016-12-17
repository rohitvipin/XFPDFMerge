using Xamarin.Forms;
using XFPDFMerge.ViewModels;

namespace XFPDFMerge.Views
{
    public partial class HomeView : ContentPage
    {
        private readonly HomeViewModel _homeViewModel;

        public HomeView(HomeViewModel homeViewModel)
        {
            BindingContext = _homeViewModel = homeViewModel;
            InitializeComponent();
        }
    }
}

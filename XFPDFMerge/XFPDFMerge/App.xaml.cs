using Xamarin.Forms;
using XFPDFMerge.ViewModels;
using XFPDFMerge.Views;

namespace XFPDFMerge
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new HomeView(new HomeViewModel());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

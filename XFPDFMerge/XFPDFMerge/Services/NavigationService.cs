using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFPDFMerge.Services
{
    public static class NavigationService
    {
        public static async Task<bool> PushAsync(Page page)
        {
            var navigationPage = Application.Current.MainPage as NavigationPage;
            if (navigationPage == null)
            {
                return false;
            }
            await navigationPage.PushAsync(page);
            return true;
        }
    }
}
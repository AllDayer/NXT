using NXTWebService.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class PopupIconPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PopupIconPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //absLayout.RaiseChild(CloseImage);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // Method for animation child in PopupPage
        // Invoked before custom animation begin
        protected async override Task OnDisappearingAnimationBegin()
        {
            await Content.FadeTo(1);
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent hide popup
            //return base.OnBackButtonPressed();
            return true;
        }

        private void OnCloseButtonTapped(object sender, System.EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
        }
    }
}

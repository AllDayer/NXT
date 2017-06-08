using NXTWebService.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class PopupAddUser : Rg.Plugins.Popup.Pages.PopupPage
    {
        public event System.EventHandler<UserDto> CallbackEvent;

        public PopupAddUser()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            absLayout.RaiseChild(CloseImage);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // Method for animation child in PopupPage
        // Invoced after custom animation end
        protected virtual Task OnAppearingAnimationEnd()
        {
            return Content.FadeTo(0.5);
        }

        // Method for animation child in PopupPage
        // Invoked before custom animation begin
        protected virtual Task OnDisappearingAnimationBegin()
        {
            return Content.FadeTo(1);
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

        private async void OnAdd(object sender, System.EventArgs e)
        {
            if (name.Text.Length > 0)
            {
                var user = new UserDto() { UserName = name.Text, Email = email.Text };
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                CallbackEvent?.Invoke(this, user);
            }
        }
    }
}

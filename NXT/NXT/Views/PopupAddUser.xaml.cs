using NXTWebService.Models;
using System;
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
            FrameContainer.HeightRequest = -1;
            //absLayout.RaiseChild(CloseCV);

            CloseImage.Rotation = 30;
            CloseImage.Scale = 0.3;
            CloseImage.Opacity = 0;

            myGrid.TranslationX = -10;
            myGrid.Opacity = 0;

            AddButton.Scale = 0.3;
            AddButton.Opacity = 0;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // Method for animation child in PopupPage
        // Invoced after custom animation end
        protected async override Task OnAppearingAnimationEnd()
        {
            var translateLength = 400u;
            await Task.WhenAll(
                                myGrid.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
                                myGrid.FadeTo(1),
                                CloseImage.FadeTo(1),
                                CloseImage.ScaleTo(1, easing: Easing.SpringOut),
                                CloseImage.RotateTo(0),
                                AddButton.ScaleTo(1),
                                AddButton.FadeTo(1));
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

        private async void OnAdd(object sender, System.EventArgs e)
        {
            if (!String.IsNullOrEmpty(name.Text))
            {
                var user = new UserDto() { UserName = name.Text, Email = email.Text };
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                CallbackEvent?.Invoke(this, user);
            }
        }
    }
}

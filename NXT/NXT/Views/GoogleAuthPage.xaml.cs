using NXT.ViewModels;
using System;
using System.Text;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class GoogleAuthPage : ContentPage
    {

        public GoogleAuthPage()
        {
            InitializeComponent();
            was_shown = false;
        }
        

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            //Authenticator = ((GoogleAuthPageViewModel)this.BindingContext).Auth;
        }
        public Xamarin.Auth.Authenticator Authenticator
        {
            get
            {
                return ((GoogleAuthPageViewModel)this.BindingContext).Auth;
            }
        }

        public void Authentication_Completed(object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e)
        {
            return;
        }

        public void Authentication_Error(object sender, Xamarin.Auth.AuthenticatorErrorEventArgs e)
        {
#if DEBUG
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Message   = {e.Message}");

            DisplayAlert
                (
                    "Error",
                    sb.ToString(),
                    "Close"
                );
#endif

            return;
        }

        public void Authentication_BrowsingCompleted(object sender, EventArgs e)
        {

            return;
        }

        bool was_shown = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (was_shown)
            {
                Navigation.PopAsync(true);
            }
            else
            {
                was_shown = true;
            }

            return;
        }

        protected override void OnDisappearing()
        {
            return;
        }
    }
}

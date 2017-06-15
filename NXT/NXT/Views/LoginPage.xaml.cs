using Xamarin.Forms;

namespace NXT.Views
{
    public partial class LoginPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        protected override bool OnBackButtonPressed()
        {
            // Prevent hide popup
            return true;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }
    }
}

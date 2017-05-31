using Xamarin.Forms;

namespace NXT.Views
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            //circle2.Source = "ic_account_circle_white_36dp.png";
        }
    }
}

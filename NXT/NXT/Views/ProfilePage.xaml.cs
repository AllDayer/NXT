using NXT.ViewModels;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                ((ProfilePageViewModel)BindingContext).PropertyChanged += ProfilePage_PropertyChanged; ;
            }

            GridColours.IsVisible = false;
            GridColours.Opacity = 0;
        }

        private void ProfilePage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowColours")
            {
                if (((ProfilePageViewModel)sender).ShowColours)
                {
                    GridColours.IsVisible = true;
                    GridColours.FadeTo(1, 300, Easing.CubicIn);
                }
                else
                {
                    var animation = new Animation(v => GridColours.Opacity = v, 1, 0);
                    animation.Commit(this, "FadeColours", 16, 250, Easing.CubicOut, (v, c) => this.GridColours.IsVisible = false);
                    //GridColours.FadeTo(0, 300, Easing.CubicOut);
                    //GridColours.IsVisible = false;
                }
            }
        }
    }
}

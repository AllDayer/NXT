using NXT.ViewModels;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class BuyPage : ContentPage
    {
        public BuyPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                ((BuyPageViewModel)BindingContext).PropertyChanged += BuyPage_PropertyChanged; ;
            }

            ShowTimeSL.IsVisible = false;
            ShowTimeSL.Opacity = 0;
        }

        private void BuyPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowTime")
            {
                if (((BuyPageViewModel)sender).ShowTime)
                {
                    ShowTimeSL.IsVisible = true;
                    ShowTimeSL.FadeTo(1, 300, Easing.CubicIn);
                }
                else
                {
                    var animation = new Animation(v => ShowTimeSL.Opacity = v, 1, 0);
                    animation.Commit(this, "FadeTime", 16, 250, Easing.CubicOut, (v, c) => this.ShowTimeSL.IsVisible = false);
                }
            }
        }
    }
}

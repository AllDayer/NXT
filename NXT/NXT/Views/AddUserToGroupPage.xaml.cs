using NXT.ViewModels;
using NXTWebService.Models;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class AddUserToGroupPage : ContentPage
    {
        public AddUserToGroupPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                friends.ItemSelected += Friends_ItemSelected;
                viewContacts.Clicked += ViewContacts_Clicked;
                ((AddUserToGroupPageViewModel)BindingContext).PropertyChanged += AddUserToGroupPage_PropertyChanged;
            }
        }

        private void AddUserToGroupPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RunActivity")
            {
                if (!((AddUserToGroupPageViewModel)sender).RunActivity)
                {
                    friends.IsVisible = true;
                    friends.FadeTo(1, 300, Easing.CubicIn);
                }
                else
                {
                    //var animation = new Animation(v => friends.Opacity = v, 1, 0);
                    //animation.Commit(this, "FadeColours", 16, 250, Easing.CubicOut, (v, c) => this.friends.IsVisible = false);
                }
            }
            else if (e.PropertyName == "FabVisible")
            {
                if (((AddUserToGroupPageViewModel)sender).FabVisible)
                {
                    FAB.TranslateTo(0, 0, easing: Easing.CubicInOut, length: 400u);
                }
                else
                {
                    FAB.TranslateTo(0, 80, easing: Easing.CubicInOut, length: 450u);
                }
            }

        }

        private void ViewContacts_Clicked(object sender, System.EventArgs e)
        {
            friends.IsVisible = false;
            friends.Opacity = 0;
        }

        private void Friends_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((AddUserToGroupPageViewModel)BindingContext).NewUserForGroup = (UserDto)friends.SelectedItem;
            ((AddUserToGroupPageViewModel)BindingContext).SaveCommand.Execute();
        }
    }
}

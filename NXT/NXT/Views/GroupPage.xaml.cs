using Prism.Events;
using NXT.Services;
using NXT.ViewModels;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class GroupPage : ContentPage
    {
        private readonly IEventAggregator _ea;

        public GroupPage(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _ea = eventAggregator;
            GroupName.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
            //_ea.GetEvent<UserAddedToGroupEvent>().Subscribe(() => SetRepeater());
        }

        protected override void OnBindingContextChanged()
        {
            repeater.ParentVM = BindingContext;
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                ((GroupPageViewModel)BindingContext).PropertyChanged += GroupPage_PropertyChanged;
                LeaveGroup.Clicked += LeaveGroup_Clicked;
                gridExtras.Opacity = 0;
                gridExtras.TranslationX = -10;
            }
        }

        private async void LeaveGroup_Clicked(object sender, System.EventArgs e)
        {
            var action = await DisplayAlert("Leave Group?", "Are you sure you want to leave this group?", "Yes", "No");
            if (action)
            {
                ((GroupPageViewModel)this.BindingContext).OnLeaveGroupCommand();
            }
        }

        private void GroupPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowExtras")
            {
                if (((GroupPageViewModel)sender).ShowExtras)
                {
                    var translateLength = 400u;
                    gridExtras.IsVisible = true;

                    gridExtras.FadeTo(1, 300, Easing.CubicIn);
                    gridExtras.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength);
                }
                else
                {
                    var animation = new Animation(v => gridExtras.Opacity = v, 1, 0);
                    animation.Commit(this, "FadeExtras", 16, 250, Easing.CubicOut, (v, c) => this.gridExtras.IsVisible = false);
                }
            }
        }

        protected override void OnDisappearing()
        {
            _ea.GetEvent<UserAddedToGroupEvent>().Unsubscribe(null);

            base.OnDisappearing();
        }
    }
}

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

            //_ea.GetEvent<UserAddedToGroupEvent>().Subscribe(() => SetRepeater());
        }

        protected override void OnBindingContextChanged()
        {
            repeater.ParentVM = BindingContext;
            base.OnBindingContextChanged();
            GridColours.IsVisible = false;
            GridColours.Opacity = 0;
            GridIcons.IsVisible = false;
            GridIcons.Opacity = 0;

            //var tapGestureRecognizer = new TapGestureRecognizer
            //{
            //    Command = ((ShoutGroupPageViewModel)BindingContext).ClickCommand,
            //    CommandParameter = "123",
            //    NumberOfTapsRequired = 1,
            //};

            //Hello.GestureRecognizers.Add(tapGestureRecognizer);
            if (BindingContext != null)
            {
                ((GroupPageViewModel)BindingContext).PropertyChanged += ShoutGroupPage_PropertyChanged;
            }
        }

        private void ShoutGroupPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowColours")
            {
                if (((GroupPageViewModel)sender).ShowColours)
                {
                    GridColours.IsVisible = true;
                    GridColours.FadeTo(1, 300, Easing.CubicIn);
                }
                else
                {
                    var animation = new Animation(v => GridColours.Opacity = v, 1, 0);
                    animation.Commit(this, "FadeColours", 16, 250, Easing.CubicOut, (v,c) => this.GridColours.IsVisible = false);
                    //GridColours.FadeTo(0, 300, Easing.CubicOut);
                    //GridColours.IsVisible = false;
                }
            }
            else if (e.PropertyName == "ShowIcons")
            {
                if (((GroupPageViewModel)sender).ShowIcons)
                {
                    GridIcons.IsVisible = true;
                    GridIcons.FadeTo(1, 300, Easing.CubicIn);
                }
                else
                {
                    var animation = new Animation(v => GridIcons.Opacity = v, 1, 0);
                    animation.Commit(this, "FadeColours", 16, 250, Easing.CubicOut, (v, c) => this.GridIcons.IsVisible = false);
                    //GridColours.FadeTo(0, 300, Easing.CubicOut);
                    //GridColours.IsVisible = false;
                }
            }
        }

        private void SetRepeater()
        {
            repeater.ParentVM = BindingContext;
            //repeater.ItemsSource = ((ShoutGroupPageViewModel)BindingContext).UsersInGroup;
        }

        protected override void OnDisappearing()
        {
            _ea.GetEvent<UserAddedToGroupEvent>().Unsubscribe(null);

            base.OnDisappearing();
        }
    }
}

using Prism.Events;
using NXT.Services;
using NXT.ViewModels;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class SummaryPage : ContentPage
    {
        private readonly IEventAggregator _ea;

        public SummaryPage(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _ea = eventAggregator;
            //_ea.GetEvent<GroupsLoadedEvent>().Subscribe(() => SetRepeater());
            //btn.BackgroundColor = Color.Transparent;
            //btn.BorderColor = Color.Transparent;
            //btn.BorderWidth = 0;
            //btn.HorizontalOptions = LayoutOptions.Center;
        }

        protected override void OnBindingContextChanged()
        {
            repeater.ParentVM = BindingContext;
            base.OnBindingContextChanged();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext != null)
            {
                ((SummaryPageViewModel)BindingContext).Login();
            }
        }

        private void SetRepeater()
        {
            //
            //repeater.ItemsSource = ((SummaryPageViewModel)this.BindingContext).ShoutGroups;
        }

        protected override void OnDisappearing()
        {
            //_ea.GetEvent<GroupsLoadedEvent>().Unsubscribe(null);

            base.OnDisappearing();
        }
    }
}

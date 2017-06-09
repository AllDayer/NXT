using DryIoc;
using Prism.DryIoc;
using NXT.Services;
using NXT.Views;
using Xamarin.Forms;

namespace NXT
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();
            CurrentApp.MainViewModel = new ViewModels.MainViewModel();

            NavigationService.NavigateAsync("/LoginPage");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterPopupNavigationService();
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<LoginPage>();
            Container.RegisterTypeForNavigation<SummaryPage>();
            Container.RegisterTypeForNavigation<BuyPage>();
            Container.RegisterTypeForNavigation<GroupPage>();
            Container.RegisterTypeForNavigation<HistoryPage>();
            Container.RegisterTypeForNavigation<PopupIconPage>();
            Container.Register<IAuthenticationService, AuthenticationService>(Reuse.Singleton);
            Container.RegisterTypeForNavigation<ProfilePage>();
            Container.RegisterTypeForNavigation<AddUserToGroupPage>();
        }
    }
}

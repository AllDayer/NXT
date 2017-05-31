using System;
using NXT.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NXT.Helpers;
using NXTWebService.Models;

namespace NXT.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        IAuthenticationService m_AuthenticationService { get; }
        IPageDialogService _pageDialogService { get; }
        public DelegateCommand OAuthCommand { get; }

        //private String TristanUserString = "d9c91004-3994-4bb4-a703-267904985126";

        public LoginPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            m_AuthenticationService = authenticationService;
            _pageDialogService = pageDialogService;

            Title = "Login";

            OAuthCommand = new DelegateCommand(OnOAuthCommandExecuted);
            if (!String.IsNullOrEmpty(Settings.Current.SocialUserID))
            {
                OnOAuthCommandExecuted();
            }
        }

        private bool m_IsLoggingIn;
        public bool IsLoggingIn
        {
            get { return m_IsLoggingIn; }
            set { SetProperty(ref m_IsLoggingIn, value); }
        }

        private async void OnOAuthCommandExecuted()
        {
            IsLoggingIn = true;
            Account account = GetFacebookAccount();
            if (account == null)
            {
                //Get auth token from Facebook
                m_AuthenticationService.RegisterFacebook(this);
                return;
            }

            await AuthenticationSuccess();

            IsLoggingIn = false;
        }

        private Account GetFacebookAccount()
        {
            IEnumerable<Account> accounts = AccountStore.Create().FindAccountsForService("Facebook");
            if (accounts != null)
            {
                return accounts.FirstOrDefault();
            }
            return null;
        }

        private async Task AuthenticationSuccess()
        {
            Account account = GetFacebookAccount();
            bool success = await m_AuthenticationService.SocialLogin(account);
            
            NavigationParameters nav = new NavigationParameters();
            var groups = await CurrentApp.MainViewModel.ServiceApi.GetGroups(Settings.Current.UserGuid.ToString());
            if (groups != null)
            {
                Settings.Current.Groups = new System.Collections.ObjectModel.ObservableCollection<GroupDto>(groups);

                await CurrentApp.MainViewModel.RefreshGroupColours();
            }

            await _navigationService.NavigateAsync("/NavigationPage/SummaryPage");
        }

        public async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            if (e.IsAuthenticated)
            {
                var accessToken = e.Account.Properties["access_token"].ToString();
                AccountStore.Create().Save(e.Account, "Facebook");
                await AuthenticationSuccess();
            }
        }

        public void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
        }

    }
}

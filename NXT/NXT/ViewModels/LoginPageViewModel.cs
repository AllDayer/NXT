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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NXT.Models.Social;

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

            var token = FbAccessToken.Current;
            if (token != null)
            {
                IsLoggingIn = true;
                System.Diagnostics.Debug.WriteLine("Token available");
                AlreadyLoggedIn(token);
            }

            OAuthCommand = new DelegateCommand(OnOAuthCommandExecuted);
            //if (!String.IsNullOrEmpty(Settings.Current.SocialUserID))
            //{
            //    OnOAuthCommandExecuted();
            //}
        }



        private bool m_IsLoggingIn;
        public bool IsLoggingIn
        {
            get { return m_IsLoggingIn; }
            set { SetProperty(ref m_IsLoggingIn, value); }
        }


        //var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture.type(large)"), null, account);
        private async void AlreadyLoggedIn(FbAccessToken token)
        {
            var res = await m_AuthenticationService.HandleAlreadyLoggedIn(token);
            if(res)
            {
                await AuthenticationSuccess();
            }
        }

        private async void OnOAuthCommandExecuted()
        {
            IsLoggingIn = true;
            bool res = await m_AuthenticationService.SocialLoginFacebook();
            if (res)
            {
                await AuthenticationSuccess();
            }
            //IsLoggingIn = true;
            //Account account = GetFacebookAccount();
            //if (account == null)
            //{
            //    //Get auth token from Facebook
            //    m_AuthenticationService.RegisterFacebook(this);
            //    return;
            //}

            //await AuthenticationSuccess();

            //IsLoggingIn = false;
        }

        //private Account GetFacebookAccount()
        //{
        //    IEnumerable<Account> accounts = AccountStore.Create().FindAccountsForService("Facebook");
        //    if (accounts != null)
        //    {
        //        return accounts.FirstOrDefault();
        //    }
        //    return null;
        //}

        private async Task AuthenticationSuccess()
        {
            //Account account = GetFacebookAccount();
            //bool success = await m_AuthenticationService.SocialLogin(account);
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
            IsLoggingIn = false;
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
        }

    }
}

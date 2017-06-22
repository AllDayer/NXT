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
        INavigationService m_NavigationService { get; }
        public DelegateCommand OAuthFacebookCommand { get; }
        public DelegateCommand OAuthTwitterCommand { get; }
        public DelegateCommand OAuthGoogleCommand { get; }
        public SummaryPageViewModel SummaryVM { get; set; }

        //private String TristanUserString = "d9c91004-3994-4bb4-a703-267904985126";

        public LoginPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            m_AuthenticationService = authenticationService;
            _pageDialogService = pageDialogService;
            m_NavigationService = navigationService;
            Title = "Login";

            if (!String.IsNullOrEmpty(Settings.Current.SocialUserID))
            {
                if (Settings.Current.UserAuth == Models.AuthType.Twitter)
                {
                    IsLoggingIn = true;
                    OnOAuthTwitterCommandExecuted();
                }
                else if (Settings.Current.UserAuth == Models.AuthType.Google)
                {
                    IsLoggingIn = true;
                    OnOAuthGoogleCommandExecuted();
                }
                else if (Settings.Current.UserAuth == Models.AuthType.Facebook)
                {
                    var token = FbAccessToken.Current;
                    if (token != null)
                    {
                        IsLoggingIn = true;
                        System.Diagnostics.Debug.WriteLine("Token available");
                        AlreadyLoggedIn(token);
                    }
                }
            }

            OAuthFacebookCommand = new DelegateCommand(OnOAuthFacebookCommandExecuted);
            OAuthTwitterCommand = new DelegateCommand(OnOAuthTwitterCommandExecuted);
            OAuthGoogleCommand = new DelegateCommand(OnOAuthGoogleCommandExecuted);
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
            if (res)
            {
                await AuthenticationSuccess();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                SummaryVM.OnRefreshCommand();
            }
            else
            {
                IsLoggingIn = false;
            }
        }

        private async void OnOAuthFacebookCommandExecuted()
        {
            IsLoggingIn = true;
            bool res = await m_AuthenticationService.SocialLoginFacebook();
            if (res)
            {
                await AuthenticationSuccess();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                await _navigationService.NavigateAsync("/NavigationPage/SummaryPage?refresh=1");
            }
            else
            {
                IsLoggingIn = false;
            }
        }

        private async void OnOAuthTwitterCommandExecuted()
        {
            bool isAutoLogin = await m_AuthenticationService.AutoLoginTwitter(this);
            if (isAutoLogin)
            {
                await AuthenticationSuccess();

                //await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                //await _navigationService.NavigateAsync("/NavigationPage/SummaryPage");
            }
        }

        private async void OnOAuthGoogleCommandExecuted()
        {
            IsLoggingIn = true;
            bool isAutoLogin = await m_AuthenticationService.AutoLoginGoogle(this);
            if (isAutoLogin)
            {
                await AuthenticationSuccess();
            }
            else
            {
                NavigationParameters nav = new NavigationParameters();
                nav.Add("auth", AuthenticationService.Authenticator);
                await m_NavigationService.NavigateAsync("GoogleAuthPage", nav);
            }
        }
        private async Task AuthenticationSuccess()
        {
            //Account account = GetFacebookAccount();
            //bool success = await m_AuthenticationService.SocialLogin(account);
            m_AuthenticationService.SetLoggedIn(true);
            NavigationParameters nav = new NavigationParameters();
            var groups = await CurrentApp.MainViewModel.ServiceApi.GetGroups(Settings.Current.UserGuid.ToString());
            if (groups != null)
            {
                Settings.Current.Groups = new System.Collections.ObjectModel.ObservableCollection<GroupDto>(groups);
            }

            if (Settings.Current.UserAuth != Models.AuthType.Facebook)
            {//Facebook doesn't like this nav
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync();
                await _navigationService.NavigateAsync("/NavigationPage/SummaryPage?refresh=1");
            }
        }

        private void AuthenticationFailure()
        {
            IsLoggingIn = false;
        }

        public async void OnGoogleAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            IsLoggingIn = true;
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnGoogleAuthCompleted;
                authenticator.Error -= OnGoogleAuthError;
            }

            if (e.IsAuthenticated)
            {
                AccountStore.Create().Save(e.Account, AuthenticationService.AccountServiceGoogle);
                await m_AuthenticationService.FetchGoogleData(e.Account);
                await AuthenticationSuccess();
            }
            else
            {
                AuthenticationFailure();
            }

        }
        public void OnGoogleAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            IsLoggingIn = false;
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnGoogleAuthCompleted;
                authenticator.Error -= OnGoogleAuthError;
            }
        }

        public async void OnTwitterAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth1Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnTwitterAuthCompleted;
                authenticator.Error -= OnTwitterAuthError;
            }

            if (e.IsAuthenticated)
            {
                await AuthenticationSuccess();
            }
            else
            {
                AuthenticationFailure();
            }
        }


        public void OnTwitterAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            IsLoggingIn = false;
            var authenticator = sender as OAuth1Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnTwitterAuthCompleted;
                authenticator.Error -= OnTwitterAuthError;
            }
        }

        public async void OnFacebookAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            IsLoggingIn = true;

            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnFacebookAuthCompleted;
                authenticator.Error -= OnFacebookAuthError;
            }

            if (e.IsAuthenticated)
            {
                AccountStore.Create().Save(e.Account, "Facebook");
                await AuthenticationSuccess();
            }
            else
            {
                AuthenticationFailure();
            }
        }

        public void OnFacebookAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            IsLoggingIn = false;
            var authenticator = sender as OAuth2Authenticator;

            if (authenticator != null)
            {
                authenticator.Completed -= OnFacebookAuthCompleted;
                authenticator.Error -= OnFacebookAuthError;
            }
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["vm"] != null)
            {
                SummaryVM = (SummaryPageViewModel)parameters["vm"];
            }
        }

    }
}

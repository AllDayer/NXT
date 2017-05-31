using System;
using Prism.Navigation;
using NXT.Helpers;
using Xamarin.Auth;
using System.Threading.Tasks;
using NXTWebService.Models;
using NXT.Models;
using NXT.ViewModels;
using System.Linq;

namespace NXT.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        INavigationService m_NavigationService { get; }

        public AuthenticationService(INavigationService navigationService)
        {
            m_NavigationService = navigationService;
        }

        public async Task<bool> SocialLogin(Account account)
        {
            return await GetFacebook(account);
        }

        #region Facebook
        async Task<bool> GetFacebook(Account account)
        {
            try
            {
                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture.type(large)"), null, account);
                var response = await request.GetResponseAsync();
                var fbUser = Newtonsoft.Json.Linq.JObject.Parse(response.GetResponseText());

                var name = fbUser["first_name"].ToString().Replace("\"", "");
                var socialID = fbUser["id"].ToString().Replace("\"", "");
                var email = fbUser["email"].ToString().Replace("\"", "");
                var avatarUrl = "";
                try
                {
                    avatarUrl = fbUser["picture"]["data"]["url"].ToString();
                    Settings.Current.AvatarUrl = avatarUrl;
                    //Settings.Current.Avatar = await CurrentApp.Current.MainViewModel.ServiceApi.GetAvatar(Settings.Current.AvatarUrl);
                }
                catch (Exception)
                { }

                UserDto userDto = null;
                
                if (!String.IsNullOrEmpty(socialID))
                {
                    userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserBySocial(socialID);
                    if (userDto != null)
                    {
                        Settings.Current.UserGuid = userDto.ID;
                    }
                }                
                
                if (userDto == null ||
                    name != userDto.UserName ||
                    socialID != userDto.SocialID ||
                    email != userDto.Email ||
                    avatarUrl != userDto.AvatarUrl)
                {
                    Settings.Current.UserFirstName = name;
                    Settings.Current.SocialUserID = socialID;
                    Settings.Current.UserEmail = email;
                    Settings.Current.UserAuth = Models.AuthType.Facebook;
                    //This is what not we have saved on the server
                    await GetOrCreate(false);
                }
                else if (name != Settings.Current.UserFirstName ||
                    socialID != Settings.Current.SocialUserID ||
                    email != Settings.Current.UserEmail ||
                    avatarUrl != Settings.Current.AvatarUrl )
                {
                    //Deleted the app
                    Settings.Current.UserFirstName = name;
                    Settings.Current.SocialUserID = socialID;
                    Settings.Current.UserEmail = email;
                    Settings.Current.AvatarUrl = avatarUrl;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task GetOrCreate(bool checkSocial = true)
        {
            bool patch = false;

            UserDto userDto = null;
            if (checkSocial && !String.IsNullOrEmpty(Settings.Current.SocialUserID))
            {
                userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserBySocial(Settings.Current.SocialUserID);
            }

            if (userDto == null)
            {
                if (!String.IsNullOrEmpty(Settings.Current.UserEmail))
                {
                    userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserByEmail(Settings.Current.UserEmail);
                }

                if (userDto == null)
                {
                    // New user
                    User u = ShoutUserFromSettings(true);
                    bool success = await CurrentApp.MainViewModel.ServiceApi.NewUser(u);
                    if (success)
                    {
                        Settings.Current.UserGuid = u.ID;
                    }
                }
                else
                {
                    // Likely created by someone else
                    patch = true;
                }
            }
            else
            {
                // Something to be updated
                patch = true;
            }

            if (patch)
            {
                User u = ShoutUserFromSettings(false);
                //if user email is found, update that
                await CurrentApp.MainViewModel.ServiceApi.PatchUser(u);
            }
        }

        private User ShoutUserFromSettings(bool newID)
        {
            User u = new User()
            {
                UserName = Settings.Current.UserFirstName,
                Email = Settings.Current.UserEmail,
                AvatarUrl = Settings.Current.AvatarUrl

            };
            if (newID)
            {
                u.ID = Guid.NewGuid();
            }
            else
            {
                u.ID = Settings.Current.UserGuid;
            }

            if (Settings.Current.UserAuth == Models.AuthType.Facebook)
            {
                u.FacebookID = Settings.Current.SocialUserID;
            }
            else if (Settings.Current.UserAuth == AuthType.Twitter)
            {
                u.TwitterID = Settings.Current.SocialUserID;
            }
            return u;
        }

        public void RegisterFacebook(LoginPageViewModel loginViewModel)
        {
            var authenticator = new OAuth2Authenticator(
                                            clientId: "842344439275386",
                                            scope: "email",
                                            authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                                            redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            authenticator.Completed += loginViewModel.OnAuthCompleted;
            authenticator.Error += loginViewModel.OnAuthError;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }


        #endregion

        public void Logout()
        {
            Settings.Current.UserName = string.Empty;
            Settings.Current.SocialUserID = string.Empty;

            System.Collections.Generic.IEnumerable<Account> accounts = AccountStore.Create().FindAccountsForService("Facebook");
            if (accounts != null)
            {
                if (accounts.FirstOrDefault() != null)
                {
                    AccountStore.Create().Delete(accounts.FirstOrDefault(), "Facebook");
                }
            }
            m_NavigationService.NavigateAsync("/LoginPage");
        }
    }
}

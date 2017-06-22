using System;
using Prism.Navigation;
using NXT.Helpers;
using Xamarin.Auth;
using System.Threading.Tasks;
using NXTWebService.Models;
using NXT.Models;
using NXT.ViewModels;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using NXT.Models.Social;
using LinqToTwitter;
using System.Text;

namespace NXT.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        INavigationService m_NavigationService { get; }
        public bool LoggedIn { get; set; }

        public const string AccountServiceTwitter = "Twitter";
        public const string AccountServiceGoogle = "Google";

        public static OAuth2Authenticator Authenticator;

        //462990388141-frj1oh3oievj4181rjndbe67ufvk09qv.apps.googleusercontent.com

        public AuthenticationService(INavigationService navigationService)
        {
            m_NavigationService = navigationService;
            parameters.Add("fields", "name, email,first_name,last_name,gender,picture.type(large)");
            LoggedIn = false;
        }

        #region Google
        public async Task<bool> AutoLoginGoogle(LoginPageViewModel loginViewModel)
        {
            var account = AccountStore.Create().FindAccountsForService(AccountServiceGoogle).FirstOrDefault();
            if (account != null)
            {
                await FetchGoogleData(account);
                return true;
            }

            Authenticator = new OAuth2Authenticator("194751005342-opqbbaotl37a6ppn8642mj0j57tiic0b.apps.googleusercontent.com",
                                                        null,
                                                        "openid email",
                                                        authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
                                                        accessTokenUrl: new Uri("https://accounts.google.com/o/oauth2/token"),
                                                        redirectUrl: new Uri("com.googleusercontent.apps.194751005342-opqbbaotl37a6ppn8642mj0j57tiic0b:/oauth2redirect"),
                                                        getUsernameAsync: null,
                                                        isUsingNativeUI: true);

            //Authenticator.Completed +=
            //    async (s, a) =>
            //    {
            //        AccountStore.Create().Save(a.Account, AccountServiceGoogle);
            //        await FetchGoogleData(account, loginViewModel);
            //        loginViewModel.OnGoogleAuthCompleted(s, a);
            //        return;
            //    };

            Authenticator.Completed += loginViewModel.OnGoogleAuthCompleted;
            Authenticator.Error += loginViewModel.OnGoogleAuthError;

            return false;
        }

        private async Task<SocialModel> GetGoogleData()
        {
            SocialModel model = new SocialModel();
            var account = AccountStore.Create().FindAccountsForService(AccountServiceGoogle).FirstOrDefault();

            var request = new OAuth2Request("GET", new Uri("https://www.googleapis.com/oauth2/v2/userinfo"), null, account);
            var response = await request.GetResponseAsync();
            if (response != null)
            {
                var userJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.GetResponseText());
                model.Id = userJson["id"];
                model.Email = userJson["email"];
                model.Name = userJson["name"];
                model.AvatarUrl = userJson["picture"];
                model.AuthType = AuthType.Google;
            }
            return model;
        }

        public async Task FetchGoogleData(Xamarin.Auth.Account account)
        {
            try
            {
                var model = await GetGoogleData();
                await RegisterAndUpdate(model);
            }
            catch (Exception) { }
            return;
        }
        #endregion

        #region Twitter
        // 
        public async Task<bool> AutoLoginTwitter(LoginPageViewModel loginViewModel)
        {
            var account = AccountStore.Create().FindAccountsForService(AccountServiceTwitter).FirstOrDefault();
            if (account != null)
            {
                await FetchTwitterData(account, loginViewModel);
                return true;
            }

            var authenticator = new OAuth1Authenticator("FWinTbxeDRfqL6pd6TyvBWqvY",
                                                        "vqHOpCafR8eNNy140xfNRy80W2zyN0A4whM9lcVWj9oSPalDAz",
                                                        new Uri("https://api.twitter.com/oauth/request_token"),
                                                        new Uri("https://api.twitter.com/oauth/authorize"),
                                                        new Uri("https://api.twitter.com/oauth/access_token"),
                                                        new Uri("https://mobile.twitter.com/home"),
                                                        async (IDictionary<string, string> accountProperties) =>
                                                        {
                                                            string screen_name = "";
                                                            if (accountProperties.TryGetValue("screen_name", out screen_name))
                                                            {
                                                                Xamarin.Auth.Account a = new Xamarin.Auth.Account(screen_name, accountProperties);
                                                                AuthenticatorCompletedEventArgs e = new AuthenticatorCompletedEventArgs(a);
                                                                AccountStore.Create().Save(a, AccountServiceTwitter);
                                                                await FetchTwitterData(a, loginViewModel);
                                                            }
                                                            return null;
                                                        });

            authenticator.Completed += loginViewModel.OnTwitterAuthCompleted;
            authenticator.Error += loginViewModel.OnTwitterAuthError;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
            return false;
        }

        private async Task FetchTwitterData(Xamarin.Auth.Account account, LoginPageViewModel loginViewModel)
        {
            try
            {
                var model = await GetTwitterData();
                await RegisterAndUpdate(model);
            }
            catch (Exception) { }
            return;
        }


        private async Task<SocialModel> GetTwitterData()
        {
            SocialModel model = new SocialModel();
            var account = AccountStore.Create().FindAccountsForService(AccountServiceTwitter).FirstOrDefault();
            var auth = new LinqToTwitter.SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = "FWinTbxeDRfqL6pd6TyvBWqvY",
                    ConsumerSecret = "vqHOpCafR8eNNy140xfNRy80W2zyN0A4whM9lcVWj9oSPalDAz",
                    AccessToken = account.Properties["oauth_token"],
                    AccessTokenSecret = account.Properties["oauth_token_secret"],
                }
            };

            var twitterCtx = new LinqToTwitter.TwitterContext(auth);

            try
            {
                var verifyResponse =
                    await
                        (from acct in twitterCtx.Account
                         where acct.Type == LinqToTwitter.AccountType.VerifyCredentials && (acct.IncludeEmail == true)
                         select acct)
                        .SingleOrDefaultAsync();

                if (verifyResponse != null && verifyResponse.User != null)
                {
                    LinqToTwitter.User user = verifyResponse.User;
                    model.Email = user.Email;
                    model.Id = user.UserIDResponse;
                    model.AvatarUrl = user.ProfileImageUrl.Replace("normal", "bigger");
                    model.AuthType = AuthType.Twitter;
                    if (!String.IsNullOrEmpty(user.Name))
                    {
                        model.Name = user.Name;
                    }
                    else if (!String.IsNullOrEmpty(user.ScreenNameResponse))
                    {
                        model.Name = user.ScreenNameResponse;
                    }
                    return model;
                }
            }
            catch (LinqToTwitter.TwitterQueryException)
            {
            }

            return model;
        }
        #endregion

        #region Facebook

        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        private string[] permissions = new string[] {
            "public_profile",
            "email",
            "user_friends"
        };


        private SocialModel GetFBData(JObject fbUser)
        {
            SocialModel fbModel = new SocialModel();
            fbModel.Name = fbUser["first_name"].ToString().Replace("\"", "");
            fbModel.Id = fbUser["id"].ToString().Replace("\"", "");
            fbModel.Email = fbUser["email"].ToString().Replace("\"", "");
            var avatarUrl = "";
            try
            {
                fbModel.AvatarUrl = fbUser["picture"]["data"]["url"].ToString();
                Helpers.Settings.Current.AvatarUrl = avatarUrl;
                //Settings.Current.Avatar = await CurrentApp.Current.MainViewModel.ServiceApi.GetAvatar(Settings.Current.AvatarUrl);
            }
            catch (Exception)
            { }

            return fbModel;
        }

        public async Task<bool> HandleAlreadyLoggedIn(FbAccessToken token)
        {
            IGraphRequest request = Xamarin.Forms.DependencyService.Get<IGraphRequest>().NewRequest(token, "/me", parameters);
            IGraphResponse response = await request.ExecuteAsync();

            if (response.RawResponse != null)
            {
                SocialModel socialModel = GetFBData(JObject.Parse(response.RawResponse));
                socialModel.AuthType = AuthType.Facebook;
                await RegisterAndUpdate(socialModel);
                return true;
            }
            return false;
        }

        public async Task<bool> SocialLoginFacebook()
        {
            try
            {
                FbAccessToken token = await Xamarin.Forms.DependencyService.Get<IFacebookLogin>().LogIn(permissions);
                if (token != null)
                {
                    // save token
                    await token.Save();

                    IGraphRequest request = Xamarin.Forms.DependencyService.Get<IGraphRequest>().NewRequest(token, "/me", parameters);
                    IGraphResponse response = await request.ExecuteAsync();
                    SocialModel socialModel = GetFBData(JObject.Parse(response.RawResponse));
                    socialModel.AuthType = AuthType.Facebook;
                    await RegisterAndUpdate(socialModel);
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("======================token was null");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            return false;
        }


        public void RegisterFacebook(LoginPageViewModel loginViewModel)
        {
            var authenticator = new OAuth2Authenticator(
                                            clientId: "842344439275386",
                                            scope: "email",
                                            authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                                            redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            authenticator.Completed += loginViewModel.OnFacebookAuthCompleted;
            authenticator.Error += loginViewModel.OnFacebookAuthError;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }
        #endregion

        public async Task GetOrCreate(bool checkSocial = true)
        {
            bool patch = false;

            UserDto userDto = null;
            if (checkSocial && !String.IsNullOrEmpty(Helpers.Settings.Current.SocialUserID))
            {
                userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserBySocial(Helpers.Settings.Current.SocialUserID, Helpers.Settings.Current.UserAuth);//Userauth isn't defaulted
            }

            if (userDto == null)
            {
                if (!String.IsNullOrEmpty(Helpers.Settings.Current.UserEmail))
                {
                    userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserByEmail(Helpers.Settings.Current.UserEmail);
                }

                if (userDto == null)
                {
                    // New user
                    Models.User u = ShoutUserFromSettings(true);
                    bool success = await CurrentApp.MainViewModel.ServiceApi.NewUser(u);
                    if (success)
                    {
                        Helpers.Settings.Current.UserGuid = u.ID;
                    }
                }
                else
                {
                    // Likely created by someone else
                    Helpers.Settings.Current.UserGuid = userDto.ID;
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
                Models.User u = ShoutUserFromSettings(false);
                //if user email is found, update that
                await CurrentApp.MainViewModel.ServiceApi.PatchUser(u);
            }
        }

        private async Task RegisterAndUpdate(SocialModel model)
        {
            //Need to check if they have changed their name and want it changed
            try
            {
                UserDto userDto = null;

                if (!String.IsNullOrEmpty(model.Id))
                {
                    userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserBySocial(model.Id, model.AuthType);
                    if (userDto != null)
                    {
                        Helpers.Settings.Current.UserGuid = userDto.ID;
                    }
                }

                if (userDto == null ||
                    model.Name != userDto.UserName ||
                    model.Id != userDto.SocialID ||
                    model.Email != userDto.Email ||
                    model.AvatarUrl != userDto.AvatarUrl)
                {
                    Helpers.Settings.Current.UserFirstName = model.Name;
                    Helpers.Settings.Current.UserName = model.Name;
                    Helpers.Settings.Current.SocialUserID = model.Id;
                    Helpers.Settings.Current.UserEmail = model.Email;
                    Helpers.Settings.Current.AvatarUrl = model.AvatarUrl;
                    Helpers.Settings.Current.UserAuth = model.AuthType;
                    //This is what not we have saved on the server
                    await GetOrCreate(false);
                }
                else if (model.Name != Helpers.Settings.Current.UserFirstName ||
                    model.Id != Helpers.Settings.Current.SocialUserID ||
                    model.Email != Helpers.Settings.Current.UserEmail ||
                    model.AvatarUrl != Helpers.Settings.Current.AvatarUrl)
                {
                    //Deleted the app
                    Helpers.Settings.Current.UserFirstName = model.Name;
                    Helpers.Settings.Current.UserName = model.Name;
                    Helpers.Settings.Current.SocialUserID = model.Id;
                    Helpers.Settings.Current.UserEmail = model.Email;
                    Helpers.Settings.Current.AvatarUrl = model.AvatarUrl;
                    Helpers.Settings.Current.UserAuth = model.AuthType;
                }
            }
            catch (Exception)
            {
            }
            return;
        }

        private Models.User ShoutUserFromSettings(bool newID)
        {

            Models.User u = new Models.User()
            {
                UserName = Helpers.Settings.Current.UserName,
                Email = Helpers.Settings.Current.UserEmail,
                AvatarUrl = Helpers.Settings.Current.AvatarUrl,
                Colour = Helpers.Settings.Current.Colour
            };
            if (newID)
            {
                u.ID = Guid.NewGuid();
                u.Colour = CurrentApp.MainViewModel.RandomColour();
            }
            else
            {
                u.ID = Helpers.Settings.Current.UserGuid;
            }

            if (Helpers.Settings.Current.UserAuth == Models.AuthType.Facebook)
            {
                u.FacebookID = Helpers.Settings.Current.SocialUserID;
            }
            else if (Helpers.Settings.Current.UserAuth == AuthType.Twitter)
            {
                u.TwitterID = Helpers.Settings.Current.SocialUserID;
            }
            else if (Helpers.Settings.Current.UserAuth == AuthType.Google)
            {
                u.GoogleID = Helpers.Settings.Current.SocialUserID;
            }
            return u;
        }

        public async void Logout()
        {
            Helpers.Settings.Current.UserName = string.Empty;
            Helpers.Settings.Current.SocialUserID = string.Empty;
            if (Helpers.Settings.Current.UserAuth == AuthType.Facebook)
            {
                await FbAccessToken.Current.Logout();
                FbAccessToken.Clear();

                var account = AccountStore.Create().FindAccountsForService("Facebook").FirstOrDefault();
                if (account != null)
                {
                    AccountStore.Create().Delete(account, "Facebook");
                }
            }
            if (Helpers.Settings.Current.UserAuth == AuthType.Twitter)
            {
                var account = AccountStore.Create().FindAccountsForService(AccountServiceTwitter).FirstOrDefault();
                if (account != null)
                {
                    AccountStore.Create().Delete(account, AccountServiceTwitter);
                }
            }
            if (Helpers.Settings.Current.UserAuth == AuthType.Google)
            {
                var account = AccountStore.Create().FindAccountsForService(AccountServiceGoogle).FirstOrDefault();
                if (account != null)
                {
                    AccountStore.Create().Delete(account, AccountServiceGoogle);
                }
            }
            await m_NavigationService.NavigateAsync("/LoginPage");
        }

        public bool IsLoggedIn()
        {
            return LoggedIn;
        }

        public void SetLoggedIn(bool val)
        {
            LoggedIn = val;
        }
    }
}

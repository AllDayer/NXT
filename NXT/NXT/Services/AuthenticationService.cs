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

namespace NXT.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        INavigationService m_NavigationService { get; }

        public AuthenticationService(INavigationService navigationService)
        {
            m_NavigationService = navigationService;
            parameters.Add("fields", "name, email,first_name,last_name,gender,picture.type(large)");
        }

        //public async Task<bool> SocialLogin(Account account)
        //{
        //    //return await GetFacebook(account);
        //}

        #region Facebook

        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        private string[] permissions = new string[] {
            "public_profile",
            "email",
            "user_friends"
        };


        private FacebookModel GetFBData(JObject fbUser)
        {
            FacebookModel fbModel = new FacebookModel();
            fbModel.Name = fbUser["first_name"].ToString().Replace("\"", "");
            fbModel.Id = fbUser["id"].ToString().Replace("\"", "");
            fbModel.Email = fbUser["email"].ToString().Replace("\"", "");
            var avatarUrl = "";
            try
            {
                fbModel.AvatarUrl = fbUser["picture"]["data"]["url"].ToString();
                Settings.Current.AvatarUrl = avatarUrl;
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

            FacebookModel deserialized = GetFBData(JObject.Parse(response.RawResponse));
            return await RegisterAndUpdate(deserialized);
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
                    FacebookModel deserialized = GetFBData(JObject.Parse(response.RawResponse));
                    return await RegisterAndUpdate(deserialized);

                    //request = request.NewRequest(token, "/me/friends", null);
                    //response = await request.ExecuteAsync();


                    //FacebookProfile profile = new FacebookProfile(deserialized["name"], deserialized["email"], deserialized["id"]);

                    //System.Diagnostics.Debug.WriteLine("--------------------popping----------------");
                    //await CoreMethods.PopToRoot(true);
                    //System.Diagnostics.Debug.WriteLine("--------------------pushing----------------");

                    //await CoreMethods.PushPageModel<LoggedInPageModel>(profile);

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

        private async Task<bool> RegisterAndUpdate(FacebookModel model)
        {
            //Need to check if they have changed their name and want it changed
            try
            {
                UserDto userDto = null;

                if (!String.IsNullOrEmpty(model.Id))
                {
                    userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserBySocial(model.Id);
                    if (userDto != null)
                    {
                        Settings.Current.UserGuid = userDto.ID;
                    }
                }

                if (userDto == null ||
                    model.Name != userDto.UserName ||
                    model.Id != userDto.SocialID ||
                    model.Email != userDto.Email ||
                    model.AvatarUrl != userDto.AvatarUrl)
                {
                    Settings.Current.UserFirstName = model.Name;
                    Settings.Current.UserName = model.Name;
                    Settings.Current.SocialUserID = model.Id;
                    Settings.Current.UserEmail = model.Email;
                    Settings.Current.AvatarUrl = model.AvatarUrl;
                    Settings.Current.UserAuth = Models.AuthType.Facebook;
                    //This is what not we have saved on the server
                    await GetOrCreate(false);
                }
                else if (model.Name != Settings.Current.UserFirstName ||
                    model.Id != Settings.Current.SocialUserID ||
                    model.Email != Settings.Current.UserEmail ||
                    model.AvatarUrl != Settings.Current.AvatarUrl)
                {
                    //Deleted the app
                    Settings.Current.UserFirstName = model.Name;
                    Settings.Current.UserName = model.Name;
                    Settings.Current.SocialUserID = model.Id;
                    Settings.Current.UserEmail = model.Email;
                    Settings.Current.AvatarUrl = model.AvatarUrl;
                    Settings.Current.UserAuth = Models.AuthType.Facebook;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //async Task<bool> GetFacebook(Account account)
        //{
        //    try
        //    {
        //        //var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture.type(large)"), null, account);
        //        //var response = await request.GetResponseAsync();
        //        //var fbUser = Newtonsoft.Json.Linq.JObject.Parse(response.GetResponseText());

        //        //var name = fbUser["first_name"].ToString().Replace("\"", "");
        //        //var socialID = fbUser["id"].ToString().Replace("\"", "");
        //        //var email = fbUser["email"].ToString().Replace("\"", "");
        //        //var avatarUrl = "";
        //        //try
        //        //{
        //        //    avatarUrl = fbUser["picture"]["data"]["url"].ToString();
        //        //    Settings.Current.AvatarUrl = avatarUrl;
        //        //    //Settings.Current.Avatar = await CurrentApp.Current.MainViewModel.ServiceApi.GetAvatar(Settings.Current.AvatarUrl);
        //        //}
        //        //catch (Exception)
        //        //{ }

        //        UserDto userDto = null;

        //        if (!String.IsNullOrEmpty(socialID))
        //        {
        //            userDto = await CurrentApp.MainViewModel.ServiceApi.GetUserBySocial(socialID);
        //            if (userDto != null)
        //            {
        //                Settings.Current.UserGuid = userDto.ID;
        //            }
        //        }                

        //        if (userDto == null ||
        //            name != userDto.UserName ||
        //            socialID != userDto.SocialID ||
        //            email != userDto.Email ||
        //            avatarUrl != userDto.AvatarUrl)
        //        {
        //            Settings.Current.UserFirstName = name;
        //            Settings.Current.SocialUserID = socialID;
        //            Settings.Current.UserEmail = email;
        //            Settings.Current.UserAuth = Models.AuthType.Facebook;
        //            //This is what not we have saved on the server
        //            await GetOrCreate(false);
        //        }
        //        else if (name != Settings.Current.UserFirstName ||
        //            socialID != Settings.Current.SocialUserID ||
        //            email != Settings.Current.UserEmail ||
        //            avatarUrl != Settings.Current.AvatarUrl )
        //        {
        //            //Deleted the app
        //            Settings.Current.UserFirstName = name;
        //            Settings.Current.SocialUserID = socialID;
        //            Settings.Current.UserEmail = email;
        //            Settings.Current.AvatarUrl = avatarUrl;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

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
                UserName = Settings.Current.UserName,
                Email = Settings.Current.UserEmail,
                AvatarUrl = Settings.Current.AvatarUrl,
                Colour = Settings.Current.Colour

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

        public async void Logout()
        {
            await FbAccessToken.Current.Logout();
            FbAccessToken.Clear();
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
            await m_NavigationService.NavigateAsync("/LoginPage");
        }
    }
}

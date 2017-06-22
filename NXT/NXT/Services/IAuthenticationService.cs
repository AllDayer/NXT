using System;
using System.Threading.Tasks;
using NXT.ViewModels;
using Xamarin.Auth;
using NXT.Models.Social;

namespace NXT.Services
{
    public interface IAuthenticationService
    {
        //Task SocialLogin(Account account);
        Task<bool> SocialLoginFacebook();
        void RegisterFacebook(LoginPageViewModel loginViewModel);
        Task<bool> HandleAlreadyLoggedIn(FbAccessToken token);
        Task FetchGoogleData(Account account);
        void Logout();
        bool IsLoggedIn();
        void SetLoggedIn(bool val);
        Task<bool> AutoLoginTwitter(LoginPageViewModel loginViewModel);
        Task<bool> AutoLoginGoogle(LoginPageViewModel loginViewModel);
    }
}

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
        void Logout();
    }
}

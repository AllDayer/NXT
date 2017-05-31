using System;
using System.Threading.Tasks;
using NXT.ViewModels;
using Xamarin.Auth;

namespace NXT.Services
{
    public interface IAuthenticationService
    {
        Task<bool> SocialLogin(Account account);
        void RegisterFacebook(LoginPageViewModel loginViewModel);
        void Logout();
    }
}

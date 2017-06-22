using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Auth;

namespace NXT.ViewModels
{
    public class GoogleAuthPageViewModel : BaseViewModel
    {
        INavigationService m_NavigationService;
        public Authenticator Auth { get; set; }
        public GoogleAuthPageViewModel(INavigationService navigationService) : base(navigationService)
        {

            m_NavigationService = navigationService;
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters["auth"] != null)
            {
                Auth = (Authenticator)parameters["auth"];
            }
        }
    }
}

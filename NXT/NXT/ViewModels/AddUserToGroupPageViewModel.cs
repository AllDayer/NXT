using NXTWebService.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NXT.ViewModels
{
    public class AddUserToGroupPageViewModel : BaseViewModel
    {
        #region properties
        INavigationService m_NavigationService;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public UserDto NewUserForGroup { get; set; }

        #endregion

        public AddUserToGroupPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            m_NavigationService = navigationService;
            NewUserForGroup = new UserDto();
            CancelCommand = new DelegateCommand(OnCancelCommand);
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        private async void OnCancelCommand()
        {
            await m_NavigationService.GoBackAsync();
        }

        private async void OnSaveCommand()
        {
            NavigationParameters nav = new NavigationParameters();
            nav.Add("user", NewUserForGroup);
            await m_NavigationService.GoBackAsync(nav);
        }

        #region navigation
        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
        #endregion
    }
}

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using NXTWebService.Models;

namespace NXT.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        INavigationService m_NavigationService;
        private GroupDto m_Group = new GroupDto();

        public GroupDto Group
        {
            get
            {
                return m_Group;
            }
            set
            {
                m_Group = value;
                RaisePropertyChanged(nameof(Group));
            }
        }

        public HistoryPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "History";
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
            Group = (GroupDto)parameters["group"];
        }
    }
}

﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using NXT.Models;
using NXT.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using NXTWebService.Models;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Prism.Events;
using NXT.Helpers;

namespace NXT.ViewModels
{
    public class SummaryPageViewModel : BaseViewModel
    {
        IAuthenticationService _authenticationService { get; }
        IEventAggregator m_EventAggregator { get; }
        public DelegateCommand LogoutCommand { get; }
        public DelegateCommand NewGroupCommand { get; }
        public DelegateCommand BuyCommand { get; }
        public DelegateCommand RefreshCommand { get; }

        private bool m_NoGroups;
        public bool NoGroups
        {
            get { return m_NoGroups; }
            set { SetProperty(ref m_NoGroups, value); }
        }

        private GroupDto m_ShoutGroupDto = new GroupDto();
        public GroupDto ShoutGroupDto
        {
            get { return m_ShoutGroupDto; }
            set { SetProperty(ref m_ShoutGroupDto, value); }
        }
        public ObservableCollection<GroupDto> Groups
        {
            get
            {
                return Settings.Current.Groups;
            }
        }


        public List<RecordDto> ShoutsForGroup { get; set; }

        public SummaryPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            Title = "NXT";
            _authenticationService = authenticationService;
            m_EventAggregator = eventAggregator;
            LogoutCommand = new DelegateCommand(OnLogoutCommandExecuted);
            NewGroupCommand = new DelegateCommand(OnNewGroupCommandExecuted);
            RefreshCommand = new DelegateCommand(OnRefreshCommand);
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            Task.Run(async () => { await LoadData(); });
            m_EventAggregator.GetEvent<GroupsLoadedEvent>().Publish();
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.GetNavigationMode() == NavigationMode.Back)
            {
                OnRefreshCommand();
            }
        }

        //private String TristanUserString = "d9c91004-3994-4bb4-a703-267904985126";

        public async Task LoadData()
        {
            //ShoutGroupDto = ShoutGroups.First();
            ShoutGroupDto = Settings.Current.Groups?.FirstOrDefault();
            if (ShoutGroupDto != null)
            {
                foreach (GroupDto sg in Settings.Current.Groups)
                {
                    //Move to a better call
                    ShoutsForGroup = await LoadShoutsForGroup(sg.ID.ToString());
                    NoGroups = false;
                }
            }
        }

        public async Task<List<RecordDto>> LoadShoutsForGroup(String groupID)
        {
            var shouts = await CurrentApp.MainViewModel.ServiceApi.GetRecordsForGroup(groupID);
            if (shouts != null)
            {
                return shouts;
            }
            return new List<RecordDto>();
        }

        public async Task RefreshGroups()
        {
            var groups = await CurrentApp.MainViewModel.ServiceApi.GetGroups(Settings.Current.UserGuid.ToString());
            Settings.Current.Groups = new System.Collections.ObjectModel.ObservableCollection<GroupDto>(groups);
            RaisePropertyChanged(nameof(Groups));
        }

        public async void OnRefreshCommand()
        {
            IsBusy = true;
            await RefreshGroups();
            await LoadData();
            IsBusy = false;
        }

        public void OnLogoutCommandExecuted() => _authenticationService.Logout();

        public async void OnNewGroupCommandExecuted()
        {
            await _navigationService.NavigateAsync("GroupPage");
        }

        public async void OnBuyCommandExecuted(BuyRoundArgs e)
        {
            var cost = e.Group.Records.Count > 0 ? e.Group.Records.Last().Cost : 0;
            var shoutID = e.Group.WhoseShout != null ? e.Group.WhoseShout.ID : Guid.Empty;
            NavigationParameters nav = new NavigationParameters();
            nav.Add("model", new RecordDto() { ID = Guid.NewGuid(), GroupID = e.Group.ID, GroupName = e.Group.Name, Cost = cost, UserID = shoutID });
            nav.Add("group", e.Group);

            await _navigationService.NavigateAsync("BuyPage", nav);
        }
    }
}

﻿using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NXT.Helpers;
using NXT.Models;
using NXT.Services;
using NXTWebService.Models;
using Xamarin.Forms;

namespace NXT.ViewModels
{
    public class GroupPageViewModel : BaseViewModel
    {

        #region properties
        INavigationService m_NavigationService;
        IEventAggregator m_EventAggregator;

        public static Guid DummyGuid = new Guid("99999999-9999-9999-9999-999999999999");

        IEnumerable<UserDto> Friends { get; set; }

        public DelegateCommand CreateGroupCommand { get; }
        //public DelegateCommand AddUserToGroupCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand ClickIcon { get; }
        public DelegateCommand ClickExtrasCommand { get; }
        public DelegateCommand LeaveGroupCommand { get; }
        public DelegateCommand<int?> UserClickedCommand { get; }
        public Command<object> ClickIconCommand { get; }
        public bool ShowIcons { get; set; }

        public ObservableCollection<UserDto> UsersInGroup { get; set; } = new ObservableCollection<UserDto>();
        public GroupDto Group { get; set; }
        public RecordDto ShoutFromEdit { get; set; }
        public bool IsEdit { get; set; } = false;
        public ObservableCollection<FileImageSource> Icons { get; set; }
        public bool ShowExtras { get; set; }


        private String m_GroupName = "";
        public String GroupName
        {
            get
            {
                return m_GroupName;
            }
            set
            {
                m_GroupName = value;
                RaisePropertyChanged(nameof(GroupName));
                CheckSubmit();
            }
        }


        private bool m_CanSubmit = false;
        public bool CanSubmit
        {
            get
            {
                return m_CanSubmit;
            }
            set
            {
                m_CanSubmit = value;
                RaisePropertyChanged(nameof(CanSubmit));
            }
        }

        private int m_SelectedIconIndex = 0;
        public int SelectedIconIndex
        {
            get
            {
                return m_SelectedIconIndex;
            }
            set
            {
                m_SelectedIconIndex = value;
                RaisePropertyChanged(nameof(SelectedIconIndex));
            }
        }

        private string m_SelectedIconName = "";
        public string SelectedIconName
        {
            get
            {
                return m_SelectedIconName;
            }
            set
            {
                m_SelectedIconName = value;
                RaisePropertyChanged(nameof(SelectedIconName));
            }
        }
        private bool m_TrackCost = true;
        public bool TrackCost
        {
            get
            {
                return m_TrackCost;
            }
            set
            {
                m_TrackCost = value;
                RaisePropertyChanged(nameof(TrackCost));
            }
        }

        public bool ShowLeaveGroup
        {
            get
            {
                return IsEdit && ShowExtras;
            }
        }

        #endregion

        public void CheckSubmit()
        {
            RaisePropertyChanged(nameof(CanSubmit));
        }

        public GroupPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            Title = "Create Group";
            m_NavigationService = navigationService;
            m_EventAggregator = eventAggregator;
            CreateGroupCommand = new DelegateCommand(OnCreateGroupCommand, CreateCommandCanExecute).ObservesProperty(() => CanSubmit);
            //AddUserToGroupCommand = new DelegateCommand(OnAddUserToGroupCommand);
            CancelCommand = new DelegateCommand(OnCancelCommand);
            ClickExtrasCommand = new DelegateCommand(OnClickExtrasCommand);
            //ClickIconCommand = new Command<object>(OnClickIconCommand);
            ClickIcon = new DelegateCommand(OnClickIcon);
            LeaveGroupCommand = new DelegateCommand(OnLeaveGroupCommand);
            UserClickedCommand = new DelegateCommand<int?>(OnUserClickedCommand);
            Icons = new ObservableCollection<FileImageSource>(CurrentApp.MainViewModel.Icons);
            UsersInGroup.CollectionChanged += UsersInGroup_CollectionChanged;
            GetFriends();
        }


        private void GetFriends()
        {
            //Should be elsewhere
            Task.Run(async () =>
            {
                Friends = await CurrentApp.MainViewModel.ServiceApi.GetFriends(Settings.Current.UserGuid);
            });
        }

        private void UsersInGroup_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CheckSubmit();
        }

        public async void OnCreateGroupCommand()
        {

            var dummy = UsersInGroup.FirstOrDefault(x => x.ID == DummyGuid);
            UsersInGroup.Remove(dummy);

            if (IsEdit)
            {
                Group.Name = GroupName;
                Group.Users = UsersInGroup.ToList();
                Group.TrackCost = TrackCost;
                Group.GroupIconName = SelectedIconName;
                await CurrentApp.MainViewModel.ServiceApi.PutGroup(Group);

                OnGoBack(true);
            }
            else
            {
                Group = new GroupDto()
                {
                    ID = Guid.NewGuid(),
                    Name = GroupName,
                    TrackCost = TrackCost,
                    GroupIconName = SelectedIconName,
                    Users = UsersInGroup.ToList()
                };

                await CurrentApp.MainViewModel.ServiceApi.CreateGroupCommand(Group);
                NavigationParameters nav = new NavigationParameters();

                if (CurrentApp.MainViewModel.GroupColourDictionary == null)
                {
                    CurrentApp.MainViewModel.GroupColourDictionary = new Dictionary<Guid, string>();
                }

                var groups = await CurrentApp.MainViewModel.ServiceApi.GetGroups(Settings.Current.UserGuid.ToString());
                nav.Add("model", groups);
                nav.Add("alert", "Group created!");
                await _navigationService.NavigateAsync("/NavigationPage/SummaryPage?refresh=1", nav);
            }
        }


        private bool CreateCommandCanExecute()
        {
            if (UsersInGroup.FirstOrDefault(x => x.ID != DummyGuid && x.ID != Settings.Current.UserGuid) != null &&
                !String.IsNullOrEmpty(GroupName))
            {
                return true;
            }
            return false;
        }

        public void OnCancelCommand()
        {
            //Show Dialog
            OnGoBack(false);
        }

        //void OnClickIconCommand(object s)
        //{
        //    SelectedIconIndex = (int)s;
        //    OnClickIcon();
        //}

        void OnClickIconNameCommand(object s)
        {
            SelectedIconName = (string)s;
            OnClickIcon();
        }

        public async void OnClickIcon()
        {
            var page = new Views.PopupAddUser();
            NavigationParameters nav = new NavigationParameters();
            nav.Add("vm", this);
            //await _navigationService.PushPopupPageAsync("PopupIcon", nav);
            await _navigationService.NavigateAsync("PopupIconPage", nav, true);
        }

        public async void OnUserClickedCommand(int? index)
        {
            if (!IsEdit && index.HasValue)
            {
                if (index == 0)
                {
                    return;
                }
                else if (index == UsersInGroup.Count - 1)
                {
                    //Push to page
                    NavigationParameters nav = new NavigationParameters();
                    nav.Add("friends", Friends);
                    await _navigationService.NavigateAsync("AddUserToGroupPage", nav);
                }
                else
                {
                    var newCollection = new ObservableCollection<UserDto>(UsersInGroup);
                    newCollection.RemoveAt(index.Value);

                    //Terrible
                    UsersInGroup.Clear();
                    foreach (var u in newCollection)
                    {
                        UsersInGroup.Add(u);
                    }
                }
            }
        }

        public void OnClickExtrasCommand()
        {
            ShowExtras = !ShowExtras;
            RaisePropertyChanged(nameof(ShowExtras));
            RaisePropertyChanged(nameof(ShowLeaveGroup));
        }

        public async void OnLeaveGroupCommand()
        {
            if (!IsBusy)
            {
                this.IsBusy = true;
            
                await CurrentApp.MainViewModel.ServiceApi.LeaveGroup(this.Group.ID.ToString(), Settings.Current.UserGuid.ToString());

                await _navigationService.NavigateAsync("/NavigationPage/SummaryPage?refresh=1");
            }
        }

        private bool IsUserInGroup(UserDto user)
        {
            if (UsersInGroup.Contains(user))
            {
                return true;
            }

            return false;
        }

        #region navigation
        private async void OnGoBack(bool refresh)
        {
            if (Group != null)
            {
                NavigationParameters nav = new NavigationParameters();
                nav.Add("group", Group);
                nav.Add("model", ShoutFromEdit);
                nav.Add("alert", "Saved");
                if (refresh)
                {
                    nav.Add("refresh", 1);
                }
                bool result = await _navigationService.GoBackAsync(nav);
            }
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["user"] != null)
            {
                var user = (UserDto)parameters["user"];
                if (user != null)
                {
                    if (!IsUserInGroup(user))
                    {
                        var newCollection = new ObservableCollection<UserDto>(UsersInGroup);
                        newCollection.Insert(UsersInGroup.Count - 1, user);

                        //Terrible
                        UsersInGroup.Clear();
                        foreach (var u in newCollection)
                        {
                            UsersInGroup.Add(u);
                        }
                    }
                }
            }
            else
            {
                if (parameters["group"] != null)
                {
                    this.Group = (GroupDto)parameters["group"];
                    RaisePropertyChanged(nameof(Group));
                    GroupName = Group.Name;
                    TrackCost = Group.TrackCost;
                    SelectedIconName = Group.GroupIconName;
                    RaisePropertyChanged(nameof(GroupName));
                    //UsersInGroup = new ObservableCollection<ShoutUserDto>(Group.Users);
                    UsersInGroup.Clear();
                    UsersInGroup = new ObservableCollection<UserDto>(Group.Users);
                    RaisePropertyChanged(nameof(UsersInGroup));
                }
                else
                {
                    UsersInGroup.Add(Settings.Current.User);

                    UserDto u = new UserDto();
                    u.ID = DummyGuid;
                    u.AvatarUrl = "ic_plus_white_18dp.png";
                    UsersInGroup.Add(u);
                }

                if (parameters["shout"] != null)
                {
                    ShoutFromEdit = (RecordDto)parameters["shout"];
                }

                if (parameters["edit"] != null)
                {
                    IsEdit = true;
                    Title = "Edit Group";
                    RaisePropertyChanged(nameof(IsEdit));
                }

            }
        }
        #endregion
    }
}

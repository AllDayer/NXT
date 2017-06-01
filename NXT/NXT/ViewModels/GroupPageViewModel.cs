using Prism.Commands;
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
        INavigationService m_NavigationService;
        IEventAggregator m_EventAggregator;

        public DelegateCommand CreateGroupCommand { get; }
        public DelegateCommand AddUserToGroupCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand ClickIcon { get; }
        public DelegateCommand<int?> RemoveUserCommand { get; }
        public Command<object> ClickIconCommand { get; }
        public bool ShowIcons { get; set; }

        public ObservableCollection<UserDto> UsersInGroup { get; set; } = new ObservableCollection<UserDto>();
        public GroupDto Group { get; set; }
        public RecordDto ShoutFromEdit { get; set; }
        public bool IsEdit { get; set; } = false;
        public ObservableCollection<FileImageSource> Icons { get; set; }

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
                RaisePropertyChanged(nameof(CanSubmit));
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

        public void CheckSubmit()
        {
            RaisePropertyChanged(nameof(CanSubmit));
        }

        public GroupPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            m_NavigationService = navigationService;
            m_EventAggregator = eventAggregator;
            CreateGroupCommand = new DelegateCommand(OnCreateGroupCommand, CreateCommandCanExecute).ObservesProperty(() => CanSubmit);
            AddUserToGroupCommand = new DelegateCommand(OnAddUserToGroupCommand);
            CancelCommand = new DelegateCommand(OnCancelCommand);

            ClickIconCommand = new Command<object>(OnClickIconCommand);
            ClickIcon = new DelegateCommand(OnClickIcon);
            RemoveUserCommand = new DelegateCommand<int?>(OnRemoveUserCommand);
            UsersInGroup.Add(new UserDto());
            Icons = new ObservableCollection<FileImageSource>(CurrentApp.MainViewModel.Icons);
        }

        public void OnAddUserToGroupCommand()
        {
            UsersInGroup.Add(new UserDto());
            RaisePropertyChanged(nameof(UsersInGroup));
        }

        public async void OnCreateGroupCommand()
        {
            if (IsEdit)
            {
                //if (CurrentApp.MainViewModel.GroupColourDictionary.ContainsKey(Group.ID))
                //{
                //    CurrentApp.MainViewModel.GroupColourDictionary[Group.ID] = SelectedColour;
                //}
                //else
                //{
                //    CurrentApp.MainViewModel.GroupColourDictionary.Add(Group.ID, SelectedColour);
                //}

                //await CurrentApp.MainViewModel.SaveGroupColours();

                Group.Name = GroupName;
                Group.Users = UsersInGroup.ToList();
                Group.TrackCost = TrackCost;
                Group.GroupIconIndex = SelectedIconIndex;
                await CurrentApp.MainViewModel.ServiceApi.PutGroup(Group);

                OnGoBack();
            }
            else
            {
                Group = new GroupDto()
                {
                    ID = Guid.NewGuid(),
                    Name = GroupName,
                    TrackCost = TrackCost,
                    Users = UsersInGroup.ToList()
                };

                Group.Users.Add(new UserDto() { ID = Settings.Current.UserGuid });

                await CurrentApp.MainViewModel.ServiceApi.CreateGroupCommand(Group);
                NavigationParameters nav = new NavigationParameters();

                if (CurrentApp.MainViewModel.GroupColourDictionary == null)
                {
                    CurrentApp.MainViewModel.GroupColourDictionary = new Dictionary<Guid, string>();
                }
                //CurrentApp.MainViewModel.GroupColourDictionary.Add(Group.ID, SelectedColour);
                await CurrentApp.MainViewModel.SaveGroupColours();

                var groups = await CurrentApp.MainViewModel.ServiceApi.GetGroups(Settings.Current.UserGuid.ToString());
                nav.Add("model", groups);
                await _navigationService.NavigateAsync("/NavigationPage/SummaryPage", nav);
            }

            await CurrentApp.MainViewModel.SaveGroupColours();
        }


        private bool CreateCommandCanExecute()
        {
            if (String.IsNullOrEmpty(GroupName))
            {
                return false;
            }

            foreach (var u in UsersInGroup)
            {
                if (String.IsNullOrEmpty(u.Email) || String.IsNullOrEmpty(u.UserName))
                {
                    return false;
                }
            }
            
            return true;
        }

        public void OnCancelCommand()
        {
            //Show Dialog
            OnGoBack();
        }

        private async void OnGoBack()
        {

            if (Group != null)
            {
                NavigationParameters nav = new NavigationParameters();
                nav.Add("group", Group);
                nav.Add("model", ShoutFromEdit);
                bool result = await _navigationService.GoBackAsync(nav);
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



        void OnClickIconCommand(object s)
        {
            SelectedIconIndex = (int)s;
            OnClickIcon();
        }

        public void OnClickIcon()
        {
            ShowIcons = !ShowIcons;
            RaisePropertyChanged(nameof(ShowIcons));
        }

        public void OnRemoveUserCommand(int? index)
        {
            if (index.HasValue)
            {
                UsersInGroup.RemoveAt(index.Value);
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

            if (parameters["group"] != null)
            {
                this.Group = (GroupDto)parameters["group"];
                RaisePropertyChanged(nameof(Group));
                GroupName = Group.Name;
                TrackCost = Group.TrackCost;
                RaisePropertyChanged(nameof(GroupName));
                //UsersInGroup = new ObservableCollection<ShoutUserDto>(Group.Users);
                UsersInGroup.Clear();
                UsersInGroup = new ObservableCollection<UserDto>(Group.Users);
                RaisePropertyChanged(nameof(UsersInGroup));
                //foreach (var u in Group.Users)
                //{
                //    UsersInGroup.Add(u);
                //}

                //if (CurrentApp.MainViewModel.GroupColourDictionary.ContainsKey(Group.ID))
                //{
                //    SelectedColour = CurrentApp.MainViewModel.GroupColourDictionary[Group.ID];
                //}
            }
            else
            {
                Random r = new Random();
                //SelectedColour = MyColours[r.Next(19)];
            }

            if (parameters["shout"] != null)
            {
                ShoutFromEdit = (RecordDto)parameters["shout"];
            }

            if (parameters["edit"] != null)
            {
                IsEdit = true;
                RaisePropertyChanged(nameof(IsEdit));
            }
        }
        //public ObservableCollection<string> MyColours = new ObservableCollection<string>() {
        //        "#c62828",//red
        //        "#ad1457",//pink
        //        "#6a1b9a",//purple
        //        "#4527a0",//deep purple
        //        "#283593",//indigo
        //        "#1565c0",//blue
        //        "#0277bd",//l blue
        //        "#00838f",//cyyan
        //        "#00695c",//teal
        //        "#2e7d32",//green
        //        "#558b2f",//l green
        //        "#9e9d24",//yello
        //        "#f9a825",//lime
        //        "#ff8f00",//amber
        //        "#ef6c00",//orange
        //        "#d84315",//deep orange
        //        "#4e342e",//Brown
        //        "#424242",//Grey
        //        "#37474f",//BlueGrey
        //    };

    }
}

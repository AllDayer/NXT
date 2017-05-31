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
        public DelegateCommand ClickColour { get; }
        public DelegateCommand ClickIcon { get; }
        public DelegateCommand<int?> RemoveUserCommand { get; }
        public Command<object> ClickColourCommand { get; }
        public Command<object> ClickIconCommand { get; }
        public bool ShowColours { get; set; }
        public bool ShowIcons { get; set; }

        public ObservableCollection<UserDto> UsersInGroup { get; set; } = new ObservableCollection<UserDto>();
        public GroupDto Group { get; set; }
        public RecordDto ShoutFromEdit { get; set; }
        public String GroupName { get; set; }
        public ObservableCollection<String> Colours { get; set; } = new ObservableCollection<String>();
        public bool IsEdit { get; set; } = false;
        public ObservableCollection<FileImageSource> Icons { get; set; }

        public GroupPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator) : base(navigationService)
        {
            Button b = new Button();
            //var x1 = (FileImageSource)ImageSource.FromFile("ic_coffee_outline_white_48dp.png");
            //var x2 = (FileImageSource)ImageSource.FromFile("ic_food_croissant_white_48dp.png");
            //Icons.Add(x1);
            //Icons.Add(x2);

            m_NavigationService = navigationService;
            m_EventAggregator = eventAggregator;
            CreateGroupCommand = new DelegateCommand(OnCreateGroupCommand);
            AddUserToGroupCommand = new DelegateCommand(OnAddUserToGroupCommand);
            CancelCommand = new DelegateCommand(OnCancelCommand);
            ClickColourCommand = new Command<object>(OnClickColourCommand);
            ClickColour = new DelegateCommand(OnClickColour);
            ClickIconCommand = new Command<object>(OnClickIconCommand);
            ClickIcon = new DelegateCommand(OnClickIcon);
            RemoveUserCommand = new DelegateCommand<int?>(OnRemoveUserCommand);
            UsersInGroup.Add(new UserDto());
            Colours = MyColours;
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
                if (CurrentApp.MainViewModel.GroupColourDictionary.ContainsKey(Group.ID))
                {
                    CurrentApp.MainViewModel.GroupColourDictionary[Group.ID] = SelectedColour;
                }
                else
                {
                    CurrentApp.MainViewModel.GroupColourDictionary.Add(Group.ID, SelectedColour);
                }

                await CurrentApp.MainViewModel.SaveGroupColours();

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

                if(CurrentApp.MainViewModel.GroupColourDictionary == null )
                {
                    CurrentApp.MainViewModel.GroupColourDictionary = new Dictionary<Guid, string>();
                }
                CurrentApp.MainViewModel.GroupColourDictionary.Add(Group.ID, SelectedColour);
                await CurrentApp.MainViewModel.SaveGroupColours();

                var groups = await CurrentApp.MainViewModel.ServiceApi.GetGroups(Settings.Current.UserGuid.ToString());
                nav.Add("model", groups);
                await _navigationService.NavigateAsync("/NavigationPage/SummaryPage", nav);
            }

            await CurrentApp.MainViewModel.SaveGroupColours();
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

        private string m_SelectedColour = "#d84315";
        public string SelectedColour
        {
            get
            {
                return m_SelectedColour;
            }
            set
            {
                m_SelectedColour = value;
                RaisePropertyChanged(nameof(SelectedColour));
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

        void OnClickColourCommand(object s)
        {
            SelectedColour = MyColours[(int)s];
            OnClickColour();
        }

        public void OnClickColour()
        {
            ShowColours = !ShowColours;
            RaisePropertyChanged(nameof(ShowColours));
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
            ShowColours = true;
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

                if (CurrentApp.MainViewModel.GroupColourDictionary.ContainsKey(Group.ID))
                {
                    SelectedColour = CurrentApp.MainViewModel.GroupColourDictionary[Group.ID];
                }
            }
            else
            {
                Random r = new Random();
                SelectedColour = MyColours[r.Next(19)];
            }

            if (parameters["shout"] != null)
            {
                ShoutFromEdit = (RecordDto)parameters["shout"];
            }

            if (parameters["edit"] != null)
            {
                IsEdit = true;
            }
        }
        public ObservableCollection<string> MyColours = new ObservableCollection<string>(CurrentApp.MainViewModel.Colours);
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

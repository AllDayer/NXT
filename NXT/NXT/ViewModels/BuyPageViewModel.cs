using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using NXT.Models;
using NXTWebService.Models;
using System.Collections.ObjectModel;

namespace NXT.ViewModels
{
    public class BuyPageViewModel : BaseViewModel
    {
        INavigationService m_NavigationService;
        private RecordDto m_Shout = new RecordDto();
        private GroupDto m_ShoutGroup = new GroupDto();

        public DelegateCommand BuyCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand EditGroupCommand { get; }
        public DelegateCommand HistoryCommand { get; }

        public ObservableCollection<UserDto> UsersForRecord { get; set; }

        public BuyPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Record";
            m_NavigationService = navigationService;
            BuyCommand = new DelegateCommand(OnBuyCommand, BuyCommandCanExecute).ObservesProperty(() => UserDto);
            CancelCommand = new DelegateCommand(OnCancelCommand);
            EditGroupCommand = new DelegateCommand(OnEditGroupCommand);
            HistoryCommand = new DelegateCommand(OnHistoryCommand);
            UsersForRecord = new ObservableCollection<UserDto>();
        }


        public String ID
        {
            get
            {
                return m_Shout.ID.ToString();
            }
            set
            {
                m_Shout.ID = new Guid(value);
                RaisePropertyChanged(nameof(ID));
            }
        }

        public String GroupID
        {
            get
            {
                return m_Shout.GroupID.ToString();
            }
        }


        public String ShoutTitle
        {
            get
            {
                return m_Shout.GroupName;
            }
        }

        public bool TrackCost
        {
            get
            {
                return m_ShoutGroup.TrackCost;
            }
        }

        public String Cost
        {
            get
            {
                return m_Shout.Cost.ToString();
            }
            set
            {
                float cost = 0;
                float.TryParse(value, out cost);
                m_Shout.Cost = cost;
                RaisePropertyChanged(nameof(Cost));
            }
        }

        private string m_UserName;
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
                if (UsersForRecord != null && UsersForRecord.Count > 0)
                {
                    UserDto = UsersForRecord.FirstOrDefault(x => x.UserName.StartsWith(value, StringComparison.OrdinalIgnoreCase));
                }
                RaisePropertyChanged(nameof(UserName));
            }
        }

        private UserDto m_UserDto;
        public UserDto UserDto
        {
            get
            {
                return m_UserDto;
            }
            set
            {
                m_UserDto = value;
                RaisePropertyChanged(nameof(UserDto));
            }
        }

        int m_SelectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return m_SelectedIndex;
            }
            set
            {
                if (m_SelectedIndex != value && value >= 0)
                {
                    m_SelectedIndex = value;
                    RaisePropertyChanged(nameof(SelectedIndex));
                    UserDto = UsersForRecord[m_SelectedIndex];
                }
            }
        }

        private bool BuyCommandCanExecute() => UserDto != null;

        public async void OnBuyCommand()
        {
            m_Shout.PurchaseTimeUtc = DateTime.UtcNow;
            m_Shout.UserID = UserDto.ID;
            m_Shout.GroupID = new Guid(GroupID);
            m_Shout.Cost = (float.Parse(Cost));

            //Save sync item
            //Sync with server
            await CurrentApp.MainViewModel.ServiceApi.NewRecord(m_Shout);
            
            //await _navigationService.NavigateAsync("SummaryPage");
            await m_NavigationService.GoBackAsync();
        }

        public async void OnCancelCommand()
        {
            //Are you sure
            //await _navigationService.NavigateAsync("SummaryPage");
            await m_NavigationService.GoBackAsync();

        }

        public async void OnEditGroupCommand()
        {
            NavigationParameters nav = new NavigationParameters();
            nav.Add("group", m_ShoutGroup);
            nav.Add("shout", m_Shout);
            nav.Add("edit", true);
            await _navigationService.NavigateAsync("GroupPage", nav);
        }

        public async void OnHistoryCommand()
        {
            NavigationParameters nav = new NavigationParameters();
            nav.Add("group", m_ShoutGroup);
            await _navigationService.NavigateAsync("HistoryPage", nav);
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        
        public override void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            m_Shout = (RecordDto)parameters["model"];
            RaisePropertyChanged(nameof(GroupID));
            RaisePropertyChanged(nameof(ID));
            RaisePropertyChanged(nameof(ShoutTitle));
            RaisePropertyChanged(nameof(Cost));

            m_ShoutGroup = (GroupDto)parameters["group"];
            RaisePropertyChanged(nameof(TrackCost));

            foreach (var u in m_ShoutGroup.Users)
            {
                UsersForRecord.Add(u);
            }

            if(m_Shout.UserID != Guid.Empty)
            {
                SelectedIndex = UsersForRecord.IndexOf(UsersForRecord.FirstOrDefault(x => x.ID == m_Shout.UserID));
            }

            RaisePropertyChanged("UserName");
        }
    }
}

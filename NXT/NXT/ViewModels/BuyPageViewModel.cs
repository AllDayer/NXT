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
        private RecordDto m_Record = new RecordDto() { PurchaseTimeUtc = DateTime.UtcNow };
        private GroupDto m_Group = new GroupDto();

        public DelegateCommand BuyCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand EditGroupCommand { get; }
        public DelegateCommand HistoryCommand { get; }
        public DelegateCommand ShowTimeCommand { get; }

        public ObservableCollection<UserDto> UsersForRecord { get; set; }

        public BuyPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Record";
            m_NavigationService = navigationService;
            BuyCommand = new DelegateCommand(OnBuyCommand, BuyCommandCanExecute).ObservesProperty(() => UserDto);
            CancelCommand = new DelegateCommand(OnCancelCommand);
            EditGroupCommand = new DelegateCommand(OnEditGroupCommand);
            HistoryCommand = new DelegateCommand(OnHistoryCommand);
            ShowTimeCommand = new DelegateCommand(OnShowTimeCommand);
            UsersForRecord = new ObservableCollection<UserDto>();
        }


        public String ID
        {
            get
            {
                return m_Record.ID.ToString();
            }
            set
            {
                m_Record.ID = new Guid(value);
                RaisePropertyChanged(nameof(ID));
            }
        }

        public String GroupID
        {
            get
            {
                return m_Record.GroupID.ToString();
            }
        }


        public String RecordTitle
        {
            get
            {
                return m_Record.GroupName;
            }
        }

        public bool TrackCost
        {
            get
            {
                return m_Group.TrackCost;
            }
        }

        public String Cost
        {
            get
            {
                return m_Record.Cost.ToString();
            }
            set
            {
                float cost = 0;
                float.TryParse(value, out cost);
                m_Record.Cost = cost;
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

        public RecordDto Record
        {
            get
            {
                return m_Record;
            }
            set
            {
                m_Record = value;
                RaisePropertyChanged(nameof(Record));
            }
        }

        public bool ShowTime { get; set; } = false;

        public TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;

        private bool BuyCommandCanExecute() => UserDto != null;

        public async void OnBuyCommand()
        {
            m_Record.UserID = UserDto.ID;
            m_Record.GroupID = new Guid(GroupID);
            m_Record.Cost = (float.Parse(Cost));
            if (m_Record.PurchaseTimeUtc == DateTime.MinValue)
            { 
                m_Record.PurchaseTimeUtc = DateTime.Today.Add(Time).ToUniversalTime();
            }

            await CurrentApp.MainViewModel.ServiceApi.NewRecord(m_Record);
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
            nav.Add("group", m_Group);
            nav.Add("shout", m_Record);
            nav.Add("edit", true);
            await _navigationService.NavigateAsync("GroupPage", nav);
        }

        public async void OnHistoryCommand()
        {
            NavigationParameters nav = new NavigationParameters();
            nav.Add("group", m_Group);
            await _navigationService.NavigateAsync("HistoryPage", nav);
        }

        public void OnShowTimeCommand()
        {
            ShowTime = !ShowTime;
            RaisePropertyChanged(nameof(ShowTime));
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        
        public override void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            m_Record = (RecordDto)parameters["model"];
            RaisePropertyChanged(nameof(GroupID));
            RaisePropertyChanged(nameof(ID));
            RaisePropertyChanged(nameof(RecordTitle));
            RaisePropertyChanged(nameof(Cost));

            m_Group = (GroupDto)parameters["group"];
            RaisePropertyChanged(nameof(TrackCost));

            foreach (var u in m_Group.Users)
            {
                UsersForRecord.Add(u);
            }

            if(m_Record.UserID != Guid.Empty)
            {
                SelectedIndex = UsersForRecord.IndexOf(UsersForRecord.FirstOrDefault(x => x.ID == m_Record.UserID));
            }

            RaisePropertyChanged("UserName");
        }
    }
}

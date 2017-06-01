using NXTWebService.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace NXT.ViewModels
{
    public class ProfilePageViewModel : BaseViewModel
    {
        private INavigationService m_NavigationService;
        public UserDto User { get; set; }
        public DelegateCommand SaveProfileCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand ClickColour { get; }
        public Command<object> ClickColourCommand { get; }
        public ObservableCollection<String> Colours { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<string> MyColours = new ObservableCollection<string>(CurrentApp.MainViewModel.Colours);
        public bool ShowColours { get; set; }


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

        private string m_Name = "";
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public ProfilePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Profile";
            m_NavigationService = navigationService;
            ClickColourCommand = new Command<object>(OnClickColourCommand);
            ClickColour = new DelegateCommand(OnClickColour);
            SaveProfileCommand = new DelegateCommand(OnSaveProfileCommand);
            CancelCommand = new DelegateCommand(OnCancelCommand);
            Colours = MyColours;
        }

        void OnClickColourCommand(object s)
        {
            SelectedColour = MyColours[(int)s];
            OnClickColour();
        }

        async void OnSaveProfileCommand()
        {
            User.Colour = SelectedColour;
            User.UserName = Name;
            await CurrentApp.MainViewModel.ServiceApi.PatchDtoUser(User);
            await m_NavigationService.GoBackAsync();
        }

        public void OnClickColour()
        {
            ShowColours = !ShowColours;
            RaisePropertyChanged(nameof(ShowColours));
        }

        async void OnCancelCommand()
        {
            await m_NavigationService.GoBackAsync();
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            User = (UserDto)parameters["model"];
            if (User.Colour != null)
            {
                SelectedColour = User.Colour;
                Name = User.UserName;
            }

            RaisePropertyChanged("UserName");
        }
    }
}

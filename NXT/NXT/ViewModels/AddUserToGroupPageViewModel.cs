using NXTWebService.Models;
using Plugin.Contacts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NXT.ViewModels
{
    public class AddUserToGroupPageViewModel : BaseViewModel
    {
        #region properties
        INavigationService m_NavigationService;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand OpenPopupCommand { get; }
        public DelegateCommand OpenContactsCommand { get; }
        public DelegateCommand PreviousContactsCommand { get; }

        public UserDto NewUserForGroup { get; set; }
        public List<UserDto> Friends { get; set; }
        public List<UserDto> ContactsByEmail { get; set; }
        public List<UserDto> PreviousContacts { get; set; }
        private bool m_FabVisible = false;
        public bool FabVisible
        {
            get
            {
                return m_FabVisible;
            }
            set
            {
                m_FabVisible = value;
                RaisePropertyChanged(nameof(FabVisible));
            }
        }

        private bool m_RunActivity = false;
        public bool RunActivity
        {
            get
            {
                return m_RunActivity;
            }
            set
            {
                m_RunActivity = value;
                RaisePropertyChanged(nameof(RunActivity));
            }
        }
        #endregion

        public AddUserToGroupPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            m_NavigationService = navigationService;
            NewUserForGroup = new UserDto();
            CancelCommand = new DelegateCommand(OnCancelCommand);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            PreviousContactsCommand = new DelegateCommand(OnPreviousContactsCommand);
            OpenPopupCommand = new DelegateCommand(OnOpenPopupCommand);
            OpenContactsCommand = new DelegateCommand(OnOpenContactsCommand);
        }

        private async void OnOpenPopupCommand()
        {
            FabVisible = false;
            var page = new Views.PopupAddUser();

            page.CallbackEvent += Page_CallbackEvent;
            page.LeavingPopUpEvent += Page_LeavingPopUpEvent;
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);
            //await Rg.Plugins.Popup.Extensions.NavigationExtension.PushPopupAsync(page);
            //await m_NavigationService.PushPopupPageAsync(page);
        }

        private void Page_LeavingPopUpEvent(object sender, EventArgs e)
        {
            FabVisible = true;
        }

        private async void OnOpenContactsCommand()
        {
            if (await CrossContacts.Current.RequestPermission())
            {
                RunActivity = true;

                CrossContacts.Current.PreferContactAggregation = false;

                await Task.Run(() =>
                {
                    if (CrossContacts.Current.Contacts == null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            RunActivity = false;
                            return;
                        });
                    }

                    var myContacts = CrossContacts.Current.Contacts.ToList()
                      .Where(c => !string.IsNullOrWhiteSpace(c.LastName) && c.Emails.Count > 0);

                    List<UserDto> ContactsByEmail = new List<UserDto>();
                    foreach(var contact in myContacts)
                    {
                        foreach(var email in contact.Emails)
                        {
                            ContactsByEmail.Add(new UserDto() { UserName = contact.DisplayName, Email = email.Address });
                        }
                    }

                    Friends = ContactsByEmail;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        RaisePropertyChanged(nameof(Friends));
                        RunActivity = false;
                    });
                });

            }
        }

        private void OnPreviousContactsCommand()
        {
            Friends = PreviousContacts;
            RaisePropertyChanged(nameof(Friends));
        }

        private void Page_CallbackEvent(object sender, UserDto e)
        {
            if (e != null && !String.IsNullOrEmpty(e.UserName))
            {
                this.NewUserForGroup = e;
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync(true);
                OnSaveCommand();
            }
        }

        private async void OnCancelCommand()
        {
            await m_NavigationService.GoBackAsync();
        }

        private async void OnSaveCommand()
        {
            NavigationParameters nav = new NavigationParameters();

            if(String.IsNullOrEmpty(NewUserForGroup.Colour))
            {
                NewUserForGroup.Colour = CurrentApp.MainViewModel.RandomColour();
            }

            nav.Add("user", NewUserForGroup);
            await m_NavigationService.GoBackAsync(nav);
        }

        #region navigation
        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters["friends"] != null)
            {
                PreviousContacts = new List<UserDto>((IEnumerable<UserDto>)parameters["friends"]);
                OnPreviousContactsCommand();
                RaisePropertyChanged(nameof(Friends));
            }
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {

        }
        #endregion
    }
}

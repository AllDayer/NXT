using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace NXT.ViewModels
{
    public class PopupIconPageViewModel : BaseViewModel
    {
        private INavigationService m_NavigationService;
        public Command<object> ClickIconCommand { get; }
        public GroupPageViewModel GroupVM { get; set; }

        public PopupIconPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            m_NavigationService = navigationService;
            ClickIconCommand = new Command<object>(OnClickIconCommand);
        }

        async void OnClickIconCommand(object s)
        {
            GroupVM.SelectedIconName = ((FileImageSource)s).File;
            //await m_NavigationService.GoBackAsync();
            await m_NavigationService.PopupGoBackAsync();
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
            if (parameters["vm"] != null)
            {
                GroupVM = (GroupPageViewModel)parameters["vm"];
                RaisePropertyChanged(nameof(GroupVM));
            }
        }
    }
}

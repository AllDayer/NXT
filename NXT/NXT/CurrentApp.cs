using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXT.ViewModels;

namespace NXT
{
    public class CurrentApp
    {
        public static  MainViewModel MainViewModel { get; set; }

        public CurrentApp()
        {
            MainViewModel = new MainViewModel();
        }

        public static void Startup()
        {
            //Intelledox.Model.ILogging log = ServiceLocator.Current.GetInstance<Intelledox.Model.ILogging>();
            //log.Info(typeof(CurrentApp), "Application startup");
            //AppVersion = new Version(0, 0);
            //ProduceVersion = new Version(0, 0);

            //MessageHub = new TinyMessenger.TinyMessengerHub();

            //// Context
            //AppContext = new ViewModel.AppContext();
            //AppContext.Options = new Common.Options();
        }
    }
}
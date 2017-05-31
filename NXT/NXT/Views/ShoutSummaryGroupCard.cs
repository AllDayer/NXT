using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhoseShoutFormsPrism.ViewModels;
using Xamarin.Forms;

namespace WhoseShoutFormsPrism.Views
{
    public partial class ShoutSummaryGroupCard : ContentView
    {
        public SummaryPageViewModel SummaryVM { get; set; }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            //BuyRound.Click += 
        }
    }
}

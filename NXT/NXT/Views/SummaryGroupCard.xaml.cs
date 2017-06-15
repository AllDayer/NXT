using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXT.Helpers;
using NXT.Services;
using NXT.ViewModels;
using NXTWebService.Models;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class SummaryGroupCard : ContentView
    {
        public SummaryPageViewModel SummaryVM { get; set; }
        public String BGColour { get; set; }
        
        public SummaryGroupCard()
        {
            InitializeComponent();
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            card.BackgroundColor = Color.FromHex(BGColour);
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                var args = new BuyRoundArgs() { Group = (GroupDto)this.BindingContext };
                SummaryVM.OnBuyCommandExecuted(args);
            };
            card.GestureRecognizers.Add(tap);
            if (BindingContext != null && ((GroupDto)BindingContext).WhoseShout != null && !String.IsNullOrEmpty(((GroupDto)BindingContext).WhoseShout.AvatarUrl))
            {
                initials.IsVisible = false;
            }

            //WebImage.Source = Settings.Current.AvatarUrl;
            //circle2.Source = Settings.Current.AvatarUrl;

            //Random r = new Random();
            //if (r.Next(2) % 2 == 0)
            //{
            //    categoryImage.Source = "ic_food_croissant_white_48dp.png";
            //}
            //BuyRound.Clicked += BuyRound_Clicked;
        }

        private void BuyRound_Clicked(object sender, EventArgs e)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXT.Services;
using NXT.ViewModels;
using NXTWebService.Models;
using Xamarin.Forms;
using NXT.Helpers;

namespace NXT.Views
{
    public partial class AddUserToGroupCard : ContentView
    {
        //Should be a view model
        public GroupPageViewModel ShoutGroupVM { get; set; }
        public String BGColor { get; set; }
        public int Index { get; set; }

        public AddUserToGroupCard()
        {
            InitializeComponent();
            name.TextChanged += TextChanged;
            email.TextChanged += TextChanged;
            
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ShoutGroupVM.CheckSubmit();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
        
    }
}

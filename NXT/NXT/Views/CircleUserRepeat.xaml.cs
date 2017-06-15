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
    public partial class CircleUserRepeat : ContentView
    {
        //Should be a view model
        public GroupPageViewModel ShoutGroupVM { get; set; }
        public String BGColor { get; set; }
        public int Index { get; set; }

        public CircleUserRepeat()
        {
            InitializeComponent();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ShoutGroupVM.CheckSubmit();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var tapGestureRecognizer = new TapGestureRecognizer
            {
                Command = ShoutGroupVM.UserClickedCommand,
                CommandParameter = Index
            };

            if (Index == 0 || ((UserDto)BindingContext).ID == GroupPageViewModel.DummyGuid || ShoutGroupVM.IsEdit)
            {
                CloseCV.IsVisible = false;
                if (((UserDto)BindingContext).ID == GroupPageViewModel.DummyGuid)
                {
                    circle.BorderThickness = 0;
                    circle.FillColor = Color.Transparent;
                }
            }
            circle.GestureRecognizers.Add(tapGestureRecognizer);

            if (BindingContext != null && !String.IsNullOrEmpty(((UserDto)BindingContext).AvatarUrl))
            {
                initials.IsVisible = false;
            }
        }
    }
}

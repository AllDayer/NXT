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
            //name.TextChanged += TextChanged;
            //email.TextChanged += TextChanged;
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
            circle.GestureRecognizers.Add(tapGestureRecognizer);
            //this.Remove.Clicked += Remove_Clicked;
            //this.Remove.SetBinding(IsVisibleProperty, new Binding("IsEdit", BindingMode.Default, new InveseBooleanConverter(), null, null, ShoutGroupVM));
        }

        private void Remove_Clicked(object sender, EventArgs e)
        {
            ShoutGroupVM.RemoveUserCommand.Execute(Index);
        }
    }
}

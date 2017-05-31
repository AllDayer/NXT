using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXT.Services;
using NXT.ViewModels;
using NXTWebService.Models;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class GridItemTemplate : ContentView
    {

        public GridItemTemplate()
        {
            InitializeComponent();
        }

        public GridItemTemplate(object item)
        {
            InitializeComponent();
            BindingContext = item;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }
        
    }
}

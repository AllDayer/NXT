using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NXT.Helpers
{
    public class IconIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((int)value == -1 || (int)value > CurrentApp.MainViewModel.Icons.Count)
            {
                return CurrentApp.MainViewModel.Icons[0];
            }
            return CurrentApp.MainViewModel.Icons[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !((bool)value);
        }
    }
}

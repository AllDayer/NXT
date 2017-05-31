using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NXT.Helpers
{
    public class DateTimeToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var datetime = (DateTime)value;
            //put your custom formatting here
            //return datetime.ToLocalTime().ToString("dddd d 0:hh:mm");
            return String.Format(new MyCustomDateProvider(), "{0}", datetime.ToLocalTime());//.ToString("dddd d 0:hh:mm");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MyCustomDateProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;

            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!(arg is DateTime)) throw new NotSupportedException();

            var dt = (DateTime)arg;

            string suffix;

            if (new[] { 11, 12, 13 }.Contains(dt.Day))
            {
                suffix = "th";
            }
            else if (dt.Day % 10 == 1)
            {
                suffix = "st";
            }
            else if (dt.Day % 10 == 2)
            {
                suffix = "nd";
            }
            else if (dt.Day % 10 == 3)
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }
            
            return string.Format("{0:dddd MMMM} {1}{2}, {0:h:mm tt}", arg, dt.Day, suffix);
        }
    }
}

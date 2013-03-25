using System;
using System.Globalization;
using System.Windows.Data;
using MySynch.Contracts.Messages;

namespace MySynch.Monitor.Utils
{
    [ValueConversion(typeof(Status), typeof(string))]
    public class StatusToIconFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return @"Icons\" + Enum.GetName(typeof(Status), value) + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

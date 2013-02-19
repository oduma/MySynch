using System;
using System.Globalization;
using System.Windows.Data;

namespace MySynch.Monitor.MVVM.ViewModels
{
    [ValueConversion(typeof(ComponentType), typeof(string))]
    public class ComponentTypeToIconFileNameConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return @"Icons\" + Enum.GetName(typeof (ComponentType), value) + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

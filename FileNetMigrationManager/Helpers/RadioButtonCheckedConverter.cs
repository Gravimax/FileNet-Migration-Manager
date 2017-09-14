using System;
using System.Windows.Data;

namespace FileNetMigrationManager
{
    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (int.Parse(value.ToString()).Equals(int.Parse(parameter.ToString())))
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}

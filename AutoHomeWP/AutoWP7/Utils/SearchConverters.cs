using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AutoWP7.Utils
{
    public class BoolToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = Visibility.Collapsed;

            bool reverse = false;
            if (parameter is string && parameter.ToString() == "1")
            {
                reverse = true;
            }

            if (value is bool? && (bool)value)
            {
                visible = reverse ? Visibility.Collapsed : Visibility.Visible;
            }
            else if (value is bool)
            {
                visible = ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
                if (reverse)
                {
                    visible = visible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class NullableBoolToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool? && (bool)value)
            {
                return true;
            }
            else
            { return false; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

    }

    /// <summary>
    /// For IEnumerable<T> null or empty convert to Collapsed, otherwise Visibility.
    /// For generic object type, null to Collapsed.
    /// </summary>
    public class EmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var enumValue = value as IEnumerable<object>;
                if (enumValue == null)
                {
                    return Visibility.Visible;
                }
                else if (enumValue.Count() > 0)
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

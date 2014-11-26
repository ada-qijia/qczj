using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                //for int type
                if (value is int)
                {
                    return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                //for Ienumerable type
                else if (value is IEnumerable<object>)
                {
                    return ((IEnumerable<object>)value).Count() > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
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

    /// <summary>
    /// Remove highlight tag B.
    /// </summary>
    public class HtmlStringToNormalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string htmlContent = value as string;
            if (!string.IsNullOrWhiteSpace(htmlContent))
            {
                htmlContent = htmlContent.Replace(@"<B>", "").Replace(@"</B>", "");
                return htmlContent;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 综合搜索车系内容块中参数配置和口碑
    /// </summary>
    public class SpecKoubeiToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEnabled=false;
            //IsShowKoubei
            if(value is int)
            {
                isEnabled = (int)value == 1;
            }
            //spec list
            else if(value is IEnumerable<object>)
            {
                isEnabled=((IEnumerable<object>)value).Count() > 0 ;
            }

            try
            {
                ResourceDictionary searchStyle = Application.Current.Resources.MergedDictionaries[1];
                return isEnabled ? searchStyle["EnabledBorderStyle"] : searchStyle["DisabledBorderStyle"];
            }
            catch { }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 综合搜索经销商模块中电话
    /// </summary>
    public class DealerTelConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           string tel=value as string;
           if(tel!=null)
           {
               return tel.StartsWith("400") ? "400电话" : "拨打电话";
           }

           return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

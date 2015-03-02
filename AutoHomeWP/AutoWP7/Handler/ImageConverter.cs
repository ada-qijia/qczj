using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ViewModels.Handler;

namespace AutoWP7
{
    public class ImageConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null && value.ToString() != "")
                {
                    ImageSource source = new StorageCachedImage(new Uri(value.ToString(), UriKind.Absolute));

                    return source;
                }
            }
            catch
            { }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

    }
    public class BrandImageConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null && value.ToString() != "")
                {
                    ImageSource source = new StorageCachedImage(new Uri(value.ToString(), UriKind.Absolute));

                    return source;

                }
                else
                {

                    return new BitmapImage(new Uri(@"Images/bg.png", UriKind.Relative));
                }
            }
            catch
            { }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

    }

    public class NewsestHeadImgConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null && value.ToString() != "")
            {
                if (value.ToString() == "头条")
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    public class NewsestShuoKeImgConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null && value.ToString() != "")
            {
                if (value.ToString() != "头条")
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}

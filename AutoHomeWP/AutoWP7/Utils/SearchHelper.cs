using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AutoWP7.Utils
{
    public enum SearchType
    {
        //综合
        [Description("综合")]
        General,
        //文章
        [Description("文章")]
        Article,
        //视频
        [Description("视频")]
        Video,
        //论坛
        [Description("论坛")]
        Forum,
        //车系
        [Description("车系")]
        Cars,
    }

    //converter
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is Enum)
            {
                Type type = value.GetType();

                MemberInfo[] memInfo = type.GetMember(value.ToString());

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        return ((DescriptionAttribute)attrs[0]).Description;
                    }
                }

                return value.ToString();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SearchHelper
    {
        public static string GetSearchPageUrlWithParams(SearchType type)
        {
            return string.Format("/View/Search/SearchPage.xaml?type={0}", (int)type);
        }

        public static bool UpdateSearchHistory(List<string> history)
        {
            throw new NotImplementedException();
        }

        //will always return a List<string>, never null.
        public static List<string> GetSearchHistory()
        {
            throw new NotImplementedException();
        }
    }
}

using CommonLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

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
        CarSeries,
    }

    public enum SuggestWordType
    {
        Video = 1,
        Forum = 2,
        Cars = 3,
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
        private const string SearchHistoryFileName = "SearchHistory.json";

        public static bool UpdateSearchHistory(IEnumerable<string> history)
        {
            string content = history == null ? "" : JsonHelper.Serialize(history);
            bool result = IsolatedStorageFileHelper.WriteIsoFile(SearchHistoryFileName, content, System.IO.FileMode.Create);
            return result;
        }

        public static List<string> GetSearchHistory()
        {
            string content = IsolatedStorageFileHelper.ReadIsoFile(SearchHistoryFileName);
            List<string> result = JsonHelper.DeserializeOrDefault<List<string>>(content);
            return result;
        }
    }

    //Set the inlines of TextBlock.
    public class InlineService
    {
        public static readonly DependencyProperty InlineListProperty = DependencyProperty.RegisterAttached("InlineList", typeof(List<Inline>), typeof(InlineService), new PropertyMetadata(null, OnInlineListPropertyChanged));

        private static void OnInlineListPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var tb = obj as TextBlock;
            if (tb != null)
            {
                tb.Inlines.Clear();
                var inlines = e.NewValue as List<Inline>;
                if (inlines != null)
                {
                    inlines.ForEach(item => tb.Inlines.Add(item));
                }
            }
        }

        public static List<Inline> GetInlineList(TextBlock element)
        {
            if (element != null)
                return element.GetValue(InlineListProperty) as List<Inline>;
            return null;
        }

        public static void SetInlineList(TextBlock element, List<Inline> inlines)
        {
            element.SetValue(InlineListProperty, inlines);
        }
    }
}

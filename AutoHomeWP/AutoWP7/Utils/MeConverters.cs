using Model.Me;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AutoWP7.Utils
{
    public class DraftTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var draftVM = value as DraftModel;
            if(draftVM!=null)
            {
                string title=string.IsNullOrEmpty(draftVM.TopicID)?"帖子："+draftVM.Title:"回帖："+draftVM.Title;
                return title;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

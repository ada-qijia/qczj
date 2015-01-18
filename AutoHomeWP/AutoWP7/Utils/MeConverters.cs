using Model.Me;
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
    public class MyInfoAuthTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return ((bool)value) ? "手机认证" : "未认证";
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DraftTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var draftVM = value as DraftModel;
            if (draftVM != null)
            {
                string title = string.IsNullOrEmpty(draftVM.TopicID) ? "帖子：" + draftVM.Title : "回帖：" + draftVM.Title;
                return title;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DateTimeToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var time = (DateTime)value;
                string format = parameter.ToString();
                return time.ToString(format);
            }
            catch
            {
                return value == null ? string.Empty : value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    //unread badge converter
    public class UnreadVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Unreadnum && parameter is string)
            {
                Unreadnum unread = value as Unreadnum;
                if (unread.Items != null)
                {
                    var found = unread.Items.FirstOrDefault(item => item.Type.ToString() == parameter.ToString());
                    if (found != null && found.Count > 0)
                    {
                        return Visibility.Visible;
                    }
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    //unread badge converter
    public class UnreadCountTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Unreadnum && parameter is string)
            {
                Unreadnum unread = value as Unreadnum;
                if (unread.Items != null)
                {
                    var found = unread.Items.FirstOrDefault(item => item.Type.ToString() == parameter.ToString());
                    if (found != null)
                    {
                        return found.Count;
                    }
                }
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #region third party binding

    public class ThirdPartyIDToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int id = (int)value;
                switch (id)
                {
                    case 2:
                        return "/Images/Me/qq.png";
                    case 8:
                        return "/Images/Me/weibo.png";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThirdPartyIDToTitleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int id = (int)value;
                switch (id)
                {
                    case 2:
                        return "腾讯QQ";
                    case 8:
                        return "新浪微博";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThirdPartyModelToUserNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = value as ThirdPartyBindingModel;
            if (model != null)
            {
                return model.RelationType == 0 ? "未绑定" : model.UserName;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThirdPartyModelToStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = value as ThirdPartyBindingModel;
            if (model != null)
            {
                switch (model.RelationType)
                {
                    case 0:
                        return "未绑定";
                    case 1:
                        return "已经绑定";
                    case 2:
                        return "重新绑定";
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThirdPartyModelToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = value as ThirdPartyBindingModel;
            if (model != null)
            {
                return model.RelationType != 1;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    public class ForumReplyToMyTopicTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Model.Me.ForumReplyModel)
            {
                var replyModel = value as ForumReplyModel;
                switch (replyModel.ReplyType)
                {
                    case 1:
                        return "回复我的帖子:" + replyModel.Title;
                    case 2:
                        return "我的回复：" + replyModel.MidReplyCotent;
                    case 3:
                        return replyModel.MidReplyName + "的回复:" + replyModel.MidReplyCotent;
                    default:
                        break;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

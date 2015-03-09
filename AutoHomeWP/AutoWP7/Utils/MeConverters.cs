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
                    case MeHelper.QQPlatformID:
                        return "/Images/Me/qq.png";
                    case MeHelper.WeiboPlatformID:
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
                    case MeHelper.QQPlatformID:
                        return "腾讯QQ";
                    case MeHelper.WeiboPlatformID:
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

    public class BindingStatusToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var status = value.ToString();
                return status.Contains("立即") || status.Contains("重新");
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    public class CommentReplyToReplyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = value as CommentReplyModel;
            if (model != null)
            {
                //bool isReplyable = model.ReplyType == 1 && model.ImgID==0;
                return model.NotAllowCom ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

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
                        return "回复我的帖子: " + replyModel.Title;
                    case 2:
                        return "我的回复: " + replyModel.MidReplyCotent;
                    case 3:
                        return replyModel.MidReplyName + "的回复: " + replyModel.MidReplyCotent;
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

    #region private message

    public class TimeStampToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double seconds;
                if (double.TryParse(value.ToString(), out seconds))
                {
                    var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var realTime = startTime.AddSeconds(seconds).ToLocalTime();
                    var now = DateTime.Now;
                    if (realTime.Date == now.Date)
                    {
                        return realTime.ToString("HH:mm");
                    }
                    else if (realTime.Year == now.Year)
                    {
                        return realTime.ToString("MM-dd");
                    }
                    else
                    {
                        return realTime.ToString("yyyy-MM-dd");
                    }
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class SendingStateToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var state = (int)value;
                return state == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}

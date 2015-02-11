using Model;
using Model.Me;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoWP7.Utils
{
    public class PrivateMessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MeItemTemplate { get; set; }

        public DataTemplate FriendItemTemplate { get; set; }

        public DataTemplate LoadMoreTemplate { get; set; }

        public static DependencyProperty FriendIDProperty = DependencyProperty.Register("FriendID", typeof(string), typeof(PrivateMessageDataTemplateSelector), null);
        public string FriendID
        {
            get { return (string)GetValue(FriendIDProperty); }
            set { SetValue(FriendIDProperty, value); }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PrivateMessageModel itemdata = item as PrivateMessageModel;
            if (itemdata != null)
            {
                if (itemdata.IsLoadMore)
                {
                    return LoadMoreTemplate;
                }
                else if (itemdata.UserID.ToString() == FriendID)
                {
                    return FriendItemTemplate;
                }
                else
                {
                    return MeItemTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }

}

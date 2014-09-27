using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoWP7.UcControl
{
    public class KoubeiDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate KoubeiTemplate { get; set; }

        public DataTemplate MoreButtonTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            KoubeiModel koubei = item as KoubeiModel;
            if (koubei != null)
            {
                if (koubei.IsMoreButton)
                {
                    return MoreButtonTemplate;
                }
                else
                {
                    return KoubeiTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}

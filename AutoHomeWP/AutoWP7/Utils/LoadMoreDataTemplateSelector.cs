using Model;
﻿using AutoWP7.UcControl;
using Model;
using System.Windows;

namespace AutoWP7.Utils
{
    public class LoadMoreDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }

        public DataTemplate LoadMoreTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            LoadMoreItem itemdata = item as LoadMoreItem;
            if (itemdata != null)
            {
                if (itemdata.IsLoadMore)
                {
                    return LoadMoreTemplate;
                }
                else
                {
                    return ItemTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}

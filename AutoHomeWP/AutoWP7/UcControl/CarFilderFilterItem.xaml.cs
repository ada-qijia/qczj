using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AutoWP7.UcControl
{
    public partial class CarFilderFilterItem : UserControl
    {
        #region DependencyProperty : FilterName

        public string FilterName
        {
            get { return (string)GetValue(FilterNameProperty); }
            set
            {
                SetValue(FilterNameProperty, value);
                filterNameTextBlock.Text = value;
            }
        }

        public static readonly DependencyProperty FilterNameProperty =
            DependencyProperty.Register("FilterName", typeof(string), typeof(CarFilderFilterItem), new PropertyMetadata(string.Empty));

        #endregion

        #region DependencyProperty : FilterValue

        public string FilterValue
        {
            get { return (string)GetValue(FilterValueProperty); }
            set
            {
                SetValue(FilterValueProperty, value);
                filterValueTextBlock.Text = value;
                if (string.IsNullOrEmpty(value))
                {
                    notSetTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    notSetTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        public static readonly DependencyProperty FilterValueProperty =
            DependencyProperty.Register("FilterValue", typeof(string), typeof(CarFilderFilterItem), new PropertyMetadata(null));

        //private static void OnItemsPanelMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    EPGList control = d as EPGList;
        //    if (control.ItemsPanel != null)
        //    {
        //        Thickness newValue = (Thickness)e.NewValue;
        //        control.ItemsPanel.Margin = newValue;
        //    }
        //}

        #endregion

        public CarFilderFilterItem()
        {
            InitializeComponent();
        }
    }
}

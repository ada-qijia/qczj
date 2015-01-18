using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class DraftBox : PhoneApplicationPage
    {
        private DraftViewModel DraftVM;

        public DraftBox()
        {
            InitializeComponent();

            this.DraftVM = DraftViewModel.SingleInstance;
            this.DataContext = this.DraftVM;

            foreach (var item in this.DraftVM.DraftList)
            {
                item.read = true;
            }
        }
    }
}
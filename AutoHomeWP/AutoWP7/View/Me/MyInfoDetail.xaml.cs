using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AutoWP7.View.Me
{
    public partial class MyInfoDetail : PhoneApplicationPage
    {
        public MyInfoDetail()
        {
            InitializeComponent();
        }

        private void Logout_Click(object sender, EventArgs e)
        {
          MessageBoxResult result= MessageBox.Show("你确定要退出登录吗？", null, MessageBoxButton.OKCancel);
            if(result==MessageBoxResult.OK)
            {
                //logout
            }
        }
    }
}
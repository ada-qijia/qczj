using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace AutoWP7.View.More
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void callAutohome_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneCallTask phoneCall = new PhoneCallTask();
            phoneCall.DisplayName = "汽车之家";
            phoneCall.PhoneNumber = "18618261181";
            phoneCall.Show();
        }


    }
}
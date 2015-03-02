using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace AutoWP7.View.More
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            this.VersionTextblock.Text = "V " + Common.GetSysVersion();
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
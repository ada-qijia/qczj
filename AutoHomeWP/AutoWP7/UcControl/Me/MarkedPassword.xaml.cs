using System.Windows;
using System.Windows.Controls;

namespace AutoWP7.UcControl.Me
{
    public partial class MarkedPassword : UserControl
    {
        public MarkedPassword()
        {
            InitializeComponent();
        }

        #region properties

        public string Password
        {
            get { return this.RealPasswordBox.Password; }
            set { this.RealPasswordBox.Password = value; }
        }

        public string Hint
        {
            get { return this.HintTextBox.Text; }
            set { this.HintTextBox.Text = value; }
        }

        #endregion

        #region watermarked passwordbox

        private void PasswordLostFocus(object sender, RoutedEventArgs e)
        {
            CheckPasswordWatermark();
        }

        public void CheckPasswordWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(this.RealPasswordBox.Password);
            this.HintTextBox.Opacity = passwordEmpty ? 100 : 0;
            this.RealPasswordBox.Opacity = passwordEmpty ? 0 : 100;
        }

        private void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            this.HintTextBox.Opacity = 0;
            this.RealPasswordBox.Opacity = 100;
        }

        #endregion
    }
}

using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ViewModels.Handler;
using ViewModels.Me;

namespace AutoWP7.UcControl.Me
{
    public partial class UpdateSelfInfo : UserControl
    {
        private CountryViewModel CountryVM;
        private CountryModel lastSelectedCountry;

        public UpdateSelfInfo()
        {
            InitializeComponent();

            this.CountryVM = CountryViewModel.SingleInstance;
            this.CountryVM.LoadDataCompleted += CountryVM_LoadDataCompleted;
            this.CountryListPicker.DataContext = this.CountryVM;

            //设置默认为中国
            var china = this.CountryVM.CountryList.FirstOrDefault(item => item.CountryCode == 86);
            if (china != null)
            {
                this.CountryListPicker.SelectedItem = china;
            }

            this.Loaded += UpdateSelfInfo_Loaded;
        }

        void UpdateSelfInfo_Loaded(object sender, RoutedEventArgs e)
        {
            //加载国家列表
            if (this.CountryVM.CountryList.Count <= 1)
            {
                this.LoadCountries();
            }
        }

        #region properties

        public bool IsPasswordVisible
        {
            get { return this.PasswordPanel.Visibility == Visibility.Visible; }
            set
            {
                this.PasswordPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                this.PasswordConfirmPanel.Visibility = this.PasswordPanel.Visibility;
            }
        }

        #endregion

        #region country list

        void CountryVM_LoadDataCompleted(object sender, EventArgs e)
        {
            var collection = this.CountryListPicker.ItemsSource as Collection<CountryModel>;
            if (lastSelectedCountry != null && collection != null)
            {
                var shouldSelected = collection.FirstOrDefault(item => item.CountryCode == lastSelectedCountry.CountryCode);
                if (shouldSelected != null)
                {
                    this.CountryListPicker.SelectedItem = shouldSelected;
                }
            }

            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
        }

        private void LoadCountries()
        {
            lastSelectedCountry = this.CountryListPicker.SelectedItem as CountryModel;

            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            this.CountryVM.LoadDataAysnc(Utils.MeHelper.GetCountryListUrl());
        }

        #endregion

        #region UI interaction

        private void ServiceProtocol_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/View/Me/ServiceProtocol.xaml", UriKind.Relative));
        }

        private void PhoneNoTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.SendCodeButton.Content = "发送验证码";
            this.SendCodeButton.IsEnabled = !string.IsNullOrEmpty(this.PhoneNoTextBox.Text);
        }

        private void SendCode_Tap(object sender, RoutedEventArgs e)
        {
            string strPattern = @"(^1[3-8]\d{9}$|^\d{3}-\d{8}$|^\d{4}-\d{7}$)";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(strPattern);
            if (!reg.IsMatch(this.PhoneNoTextBox.Text))
            {
                Common.showMsg("手机号格式不正确");
            }
            else
            {
                var selectedCountry = this.CountryListPicker.SelectedItem as CountryModel;
                if (selectedCountry != null)
                {
                    string data = string.Format("a=2&pm=3&v={0}&phonenum={1}&countrycode={2}&phonelength={3}&type=1", App.version, this.PhoneNoTextBox.Text, selectedCountry.CountryCode, selectedCountry.PhoneLength, 1);
                    string url = Utils.MeHelper.GetSendCheckCodeUrl();
                    ViewModels.Me.UpStreamViewModel upstreamVM = ViewModels.Me.UpStreamViewModel.SingleInstance;
                    upstreamVM.UploadAsync(url, data, wc_UploadStringCompleted);
                }
            }
        }

        private void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs ee)
        {
            try
            {
                if (ee.Error != null || ee.Cancelled)
                {
                    Common.showMsg("发送失败");
                }
                else
                {
                    JObject json = JObject.Parse(ee.Result);
                    int resultCode = (int)json.SelectToken("returncode");
                    string message = json.SelectToken("message").ToString();
                    switch (resultCode)
                    {
                        case 0://成功
                            this.SendCodeButton.Content = "已发送验证码";
                            this.SendCodeButton.IsEnabled = false;
                            break;
                        case 10010:
                            Common.showMsg("手机号码已注册，您可以使用此号码直接登录");
                            break;
                        case 10004:
                        case 10005:
                            Common.showMsg("号码不可用，请重新校验");
                            break;
                        default://其他情况...
                            if (!string.IsNullOrEmpty(message))
                            {
                                Common.showMsg(message);
                            }
                            break;
                    }
                }
            }
            catch
            {

            }
        }

        #endregion

        public bool CheckData()
        {
            if (string.IsNullOrEmpty(this.UsernameTextbox.Text))
            {
                Common.showMsg("请您输入用户名");
            }
            else if (this.PasswordPanel.Visibility == Visibility.Visible)
            {
                string passcode = this.PasswordPanel.Password.Trim();
                //数字，字母，符号（非中文和回车）
                //Regex reg1 = new Regex("(^[^(\u4e00-\u9fa5|\n)][6,25]$)");
                //不能全是字母，数字或符号
                Regex reg2 = new Regex(@"^((.*?\d)(?=.*?[A-Za-z])|(?=.*?\d)(?=.*?[\W_])|(?=.*?[A-Za-z])(?=.*?[\W_]))[^(\u4e00-\u9fa5|\n)]");
                if (string.IsNullOrEmpty(passcode))
                {
                    Common.showMsg("请输入密码");
                }
                else if (!reg2.IsMatch(passcode))
                {
                    Common.showMsg("密码不能为纯数字、纯字母、纯符号");
                }
                else if (passcode == this.UsernameTextbox.Text.Trim())
                {
                    Common.showMsg("密码不能和用户名相同");
                }
                else if (passcode.Length < 6 || passcode.Length > 25)
                {
                    Common.showMsg("密码应为6-25个字符");
                }
                else if (new Regex(@"(.*[\u4e00-\u9fa5]+.*)").IsMatch(passcode))
                {
                    Common.showMsg("密码不能使用中文");
                }
                else if (string.IsNullOrWhiteSpace(this.PasswordConfirmPanel.Password))
                {
                    Common.showMsg("请输入确认密码");
                }
                else if (this.PasswordConfirmPanel.Password.Trim() != passcode)
                {
                    Common.showMsg("登录密码不一致");
                }
                else
                {
                    return true;
                }
            }
            else if (string.IsNullOrEmpty(this.PhoneNoTextBox.Text))
            {
                Common.showMsg("请您输入手机号");
            }
            else if (string.IsNullOrEmpty(this.CodeTextbox.Text))
            {
                Common.showMsg("请您输入验证码");
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}

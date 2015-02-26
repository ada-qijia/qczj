using Microsoft.Phone.Controls;
using System;
using System.IO.IsolatedStorage;
using System.Windows.Navigation;

namespace AutoWP7.View.Me
{
    public partial class PushNotificationSetting : PhoneApplicationPage
    {
        public PushNotificationSetting()
        {
            InitializeComponent();
            SetPushSetting();
        }

        #region 推送设置

        private void SetPushSetting()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(Utils.PushNotificationHelper.pushSettingKey))
            {
                var pushSetting = settings[Utils.PushNotificationHelper.pushSettingKey] as Model.Me.PushNotificationSettingModel;
                if (pushSetting != null)
                {
                    this.SystemNotificationSwitch.IsChecked = !pushSetting.NotAllowSystem;
                    this.PersonalNotificationSwitch.IsChecked = !pushSetting.NotAllowPerson;
                    var today = DateTime.Now.Date;
                    this.StartTimePicker.Value = today.AddHours(pushSetting.StartTime / 100).AddMinutes(pushSetting.StartTime % 100);
                    this.EndTimePicker.Value = today.AddHours(pushSetting.EndTime / 100).AddMinutes(pushSetting.EndTime % 100);
                }
            }
        }

        private void UploadPushSetting()
        {
            var notAllowSystem = this.SystemNotificationSwitch.IsChecked.Value == false;
            var notAllowPerson = this.PersonalNotificationSwitch.IsChecked.Value == false;
            var startTime = this.StartTimePicker.Value.Value.Hour * 100 + this.StartTimePicker.Value.Value.Minute;
            var endTime = this.EndTimePicker.Value.Value.Hour * 100 + this.EndTimePicker.Value.Value.Minute;

            var userInfo = Utils.MeHelper.GetMyInfoModel();
            if (userInfo != null)
            {
                Utils.PushNotificationHelper.SaveUserSetting(userInfo.UserID.ToString(), !notAllowSystem, !notAllowPerson, startTime, endTime,SaveUserSettingCompleted);
            }
        }

        private void SaveUserSettingCompleted(object sender, bool result)
        {
            string msg=result?"保存成功":"保存失败";
            Handler.Common.showMsg(msg);
        }

        #endregion

        #region UI交互

        private void SystemNotificationSwitch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UploadPushSetting();
        }

        private void PersonalNotificationSwitch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UploadPushSetting();
        }

        private void StartTimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            UploadPushSetting();
        }

        private void EndTimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            UploadPushSetting();
        }

        #endregion
    }
}
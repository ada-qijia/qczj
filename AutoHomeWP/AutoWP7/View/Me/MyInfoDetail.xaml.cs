using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Navigation;
using ViewModels.Handler;

namespace AutoWP7.View.Me
{
    public partial class MyInfoDetail : PhoneApplicationPage
    {
        private Model.Me.MeModel model;

        public MyInfoDetail()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var appState = PhoneApplicationService.Current.State;
            if (appState.Keys.Contains(Utils.MeHelper.MyInfoStateKey) && model == null)
            {
                model = appState[Utils.MeHelper.MyInfoStateKey] as Model.Me.MeModel;
                this.DataContext = model;
                PhoneApplicationService.Current.State.Remove(Utils.MeHelper.MyInfoStateKey);
            }
        }


        private void Logout_Click(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "退出登录",
                Message = "你确定要退出登录吗？",
                LeftButtonContent = "确定",
                RightButtonContent = "取消",
                IsFullScreen = false
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        //注销推送通知
                        Utils.PushNotificationHelper.UnRegisterDevice();


                        //退出登录
                        var setting = IsolatedStorageSettings.ApplicationSettings;
                        string key = "userInfo";
                        if (setting.Contains(key))//已经登录
                        {
                            setting.Clear();
                            setting.Save();
                        }

                        //清除我的论坛
                        using (LocalDataContext ldc = new LocalDataContext())
                        {
                            try
                            {
                                var deleteAllItem = from d in ldc.myForum where d.Id > 0 select d;
                                ldc.GetTable<MyForumModel>().DeleteAllOnSubmit(deleteAllItem);
                                ldc.SubmitChanges();
                            }
                            catch
                            { }
                        }

                        NavigationService.GoBack();
                        // Common.showMsg("退出登录成功");
                        break;
                    case CustomMessageBoxResult.RightButton:
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }
    }
}
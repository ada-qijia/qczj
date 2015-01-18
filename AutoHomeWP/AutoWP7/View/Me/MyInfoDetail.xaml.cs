using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using ViewModels.Handler;
using Model;
using AutoWP7.Handler;

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
            }
        }


        private void Logout_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("你确定要退出登录吗？", null, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var setting = IsolatedStorageSettings.ApplicationSettings;
                string key = "userInfo";
                if (setting.Contains(key))//已经登录
                {
                    setting.Remove(key);
                    setting.Clear();
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
                    catch (Exception ex)
                    {

                    }
                }

                Common.showMsg("退出登录成功");
            }
        }
    }
}
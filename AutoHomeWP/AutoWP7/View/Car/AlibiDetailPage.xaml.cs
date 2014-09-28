using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using ViewModels;
using Microsoft.Phone.Tasks;
using ViewModels.Handler;

namespace AutoWP7.View.Car
{
    public partial class AlibiDetailPage : PhoneApplicationPage
    {
        string koubeiID = string.Empty;

        public AlibiDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            koubeiID = NavigationContext.QueryString["id"];

            LoadData();
        }

        #region Data

        AlibiDetailViewModel alibiVM = null;
        List<AlibiDetailPicModel> picList = new List<AlibiDetailPicModel>();
        public void LoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (alibiVM == null)
            {
                alibiVM = new AlibiDetailViewModel();
                alibiVM.LoadDataCompleted += alibiVM_LoadDataCompleted;
            }

            //string url = "http://app.api.autohome.com.cn/wpv1.4/alibi/alibiinfo-a2-pm3-v1.5.0-k257366.html";
            string url = string.Format("{0}{1}/alibi/alibiinfo-{2}-k{3}.html", App.appUrl, App.versionStr, App.AppInfo, koubeiID);
            alibiVM.LoadDataAysnc(url);
        }

        void alibiVM_LoadDataCompleted(object sender, APIEventArgs<AlibiDetailModel> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                AlibiDetailModel data = e.Result;
                picList = data.piclist;
                this.LayoutRoot.DataContext = data;

                if (data.carinfo!=null)
                {
                    var info = data.carinfo;
                    alibiChart.SetColumns(
                        info.space,
                        info.power,
                        info.maneuverability,
                        info.oilconsumption,
                        info.comfortabelness,
                        info.apperance,
                        info.inside,
                        info.costefficient);
                }
            }
        }

        #endregion

    }
}
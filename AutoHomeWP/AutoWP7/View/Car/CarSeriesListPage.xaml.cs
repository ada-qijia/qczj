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
using ViewModels;
using ViewModels.Handler;
using Model;
using System.Collections.ObjectModel;
using AutoWP7.Handler;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 车系列表页
    /// </summary>
    public partial class CarSeriesListPage : PhoneApplicationPage
    {
        public CarSeriesListPage()
        {
            InitializeComponent();
        }

        string strId = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            strId = NavigationContext.QueryString["id"];
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                CarBrandLoadData();
            }

        }


        private class CarSeriesGroup : List<CarSeriesModel>
        {
            public CarSeriesGroup(string category)
            {
                key = category;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class CarSeriesSource : List<CarSeriesGroup>
        {

        }

        CarSeriesSource carSeriesSource = new CarSeriesSource();


        #region 车系数据加载

        CarSeriesViewModel carSeriesVM = null;
        /// <summary>
        /// 车系
        /// </summary>
        public void CarBrandLoadData()
        {
            //清除表中以前的数据
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var item = from s in ldc.carSeries where s.Id > 0 select s;
                ldc.carSeries.DeleteAllOnSubmit(item);
                ldc.SubmitChanges();
            }

            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            carSeriesVM = new CarSeriesViewModel();

            string url = string.Format("{0}{2}/cars/seriesprice-a2-pm3-v{3}-b{1}-t2.html", App.appUrl, strId, App.versionStr, App.version);
            carSeriesVM.LoadDataAysnc(url);
            carSeriesVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CarSeriesModel>>>(carSeriesVM_LoadDataCompleted);

        }

        void carSeriesVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarSeriesModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                //  carBrandDataSource = (ObservableCollection<CarSeriesModel>)e.Result;

                if (e.Result.Count() < 1)
                {
                    Common.showMsg("暂无此车系数据哦");
                }
                else
                {
                    var groupBy = from car in e.Result
                                  group car by car.FctName into c
                                  select new Group<CarSeriesModel>(c.Key, c);
                
                    foreach (var entity in groupBy)
                    {
                        CarSeriesGroup group = new CarSeriesGroup(entity.key);

                        foreach (var item in entity)
                        {
                            group.Add(item);
                        }
                        carSeriesSource.Add(group);
                    }
                    carSeriesListGropus.ItemsSource = carSeriesSource;
                }

            }
        }


        

        #endregion

        //导向车系页面
        private void carSeriesGird_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;

            //共享车系
            var model = gg.DataContext as CarSeriesModel;
            Model.Me.FavoriteCarSeriesModel favoriteModel = new Model.Me.FavoriteCarSeriesModel();
            favoriteModel.ID = model.Id;
            favoriteModel.Img = model.ImgUrl;
            favoriteModel.Level = model.Level;
            favoriteModel.Name = model.Name;
            favoriteModel.PriceBetween = model.PriceBetween;
            View.Car.CarSeriesDetailPage.ShareModel(favoriteModel);

            this.NavigationService.Navigate(new Uri("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId=" + gg.Tag, UriKind.Relative));
        }

    }
}
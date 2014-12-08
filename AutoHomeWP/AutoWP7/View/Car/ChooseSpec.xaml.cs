using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViewModels;
using ViewModels.Handler;
using Model;

namespace AutoWP7.View.Car
{
    public partial class chooseSpec : PhoneApplicationPage
    {
        public chooseSpec()
        {
            InitializeComponent();
        }
        string dealerid = string.Empty;
        string cityID = string.Empty;
        string seriesID = string.Empty;
        string specID = string.Empty;
          List<SpecGroup> customers = null;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.timerId++;
            //将车型id存放于全局
            dealerid = this.NavigationContext.QueryString["dealerid"];
            cityID = this.NavigationContext.QueryString["cityID"];
            seriesID = this.NavigationContext.QueryString["seriesID"];
            specID = this.NavigationContext.QueryString["specID"];
            if (seriesID.ToString() != App.CarSeriesId)
            {
                CarSeriesQuoteLoadData();
            }
            else
            {
                using (LocalDataContext ldc = new LocalDataContext())
                {
                    var groupBy = from car in ldc.carQuotes
                                  orderby car.GroupOrder ascending, car.COrder ascending
                                  group car by new { N = car.GroupName, O = car.GroupOrder } into c
                                  orderby c.Key.O ascending
                                  select new Group<CarSeriesQuoteModel>(c.Key.N, c);
                    foreach (var item in groupBy)
                    {
                        foreach (var ii in item)
                        {
                            SpecGroup gro = new SpecGroup("选择车型：");
                            gro.Add(ii);
                            specSource.Add(gro);
                        }                            
                    }
                    specList.ItemsSource = specSource;
                }
            }
        }
        CarSeriesQuoteViewModel carSeriesQuoteVM = null;
        SpecSource specSource = new SpecSource();
        
        private class SpecSource : List<SpecGroup>
        {

        }
        public void CarSeriesQuoteLoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            //清除表中以前的数据
            using (ViewModels.Handler.LocalDataContext ldc = new ViewModels.Handler.LocalDataContext())
            {
                var item = from s in ldc.carQuotes where s.Id > 0 select s;
                ldc.carQuotes.DeleteAllOnSubmit(item);
                ldc.SubmitChanges();
            }

            if(carSeriesQuoteVM == null)
            {
                carSeriesQuoteVM = new CarSeriesQuoteViewModel();
            }
            string url = string.Format("{0}{2}/cars/specslist-a2-pm3-v1.6.0-t0x000c-ss{1}.html", App.appUrl, seriesID, App.versionStr);
            carSeriesQuoteVM.LoadDataAysnc(url);
            carSeriesQuoteVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>>>(CarSeriesQuote_LoadDataCompleted);

        }
        void CarSeriesQuote_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error == null)
            {
                LoadData(e.Result.ToList());
            }
           
        }
        LocalDataContext ldc = new LocalDataContext();
        void LoadData(IEnumerable<Model.CarSeriesQuoteModel> allSpec)
        {
           //var queryProvince = from s in ldc.carQuotes where s.Id > 0 select s;
           // if (queryProvince.Count() > 0)
           // {
                //var groupBy = (from p in queryProvince
                //               orderby p.Id
                //               select p).ToList();
                //Group<Model.CarSeriesQuoteModel> gr = new Group<Model.CarSeriesQuoteModel>("选择车型：", groupBy);
                foreach (var en in allSpec)
                {
                    SpecGroup gro = new SpecGroup("选择车型：");
                    gro.Add(en);
                    specSource.Add(gro);
                }
                specList.ItemsSource = specSource;
            //}
            
        }
       


   

        private void cityNameStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock ss = (TextBlock)sender;
            App.AskSpec = ss.Tag.ToString();
            this.NavigationService.GoBack();
        }

       
    }
    class SpecGroup : List<Model.CarSeriesQuoteModel>
    {
        public SpecGroup(string category)
        {
            key = category;
        }

        public string key { get; set; }
        public bool HasItems
        {
            get { return Count > 0; }
        }
    }
}
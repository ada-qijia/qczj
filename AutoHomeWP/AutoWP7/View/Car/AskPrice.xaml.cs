using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AutoWP7.Handler;
using ViewModels;
using Microsoft.Phone.Info;
using ViewModels.Handler;
using Model;
namespace AutoWP7.View.Car
{
    public partial class AskPrice : PhoneApplicationPage
    {
        public AskPrice()
        {
            InitializeComponent();
        }
        string dealerid = string.Empty;
        string cityID = string.Empty;
        string seriesID = string.Empty;
        string specID = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.timerId++;
            //将车型id存放于全局
            dealerid = this.NavigationContext.QueryString["dealerid"];
            cityID = this.NavigationContext.QueryString["cityID"];
            seriesID = this.NavigationContext.QueryString["seriesID"];
            specID = this.NavigationContext.QueryString["specID"];

            if (!string.IsNullOrEmpty(App.AskName))
            {
                this.iName.Text = App.AskName;
            }
            if (!string.IsNullOrEmpty(App.AskPhone))
            {
                this.iPhone.Text = App.AskPhone;
            }
            if (App.AskSpec != null)
            {
                specID = App.AskSpec;
            }

            if (seriesID != App.CarSeriesId)
                CarSeriesQuoteLoadData();
            else
            {
                using (LocalDataContext ldc = new LocalDataContext())
                {
                    var groupBy = from car in ldc.carQuotes
                                  orderby car.GroupOrder ascending, car.COrder ascending
                                  group car by new { N = car.GroupName, O = car.GroupOrder } into c
                                  orderby c.Key.O ascending
                                  select new Group<CarSeriesQuoteModel>(c.Key.N, c);
                    bool isFind = false;
                    string specname = "";
                    int specid = 0;
                    foreach (var entity in groupBy)
                    {
                        var entity1 = from c in entity orderby c.COrder ascending select c;
                        foreach (var item in entity1)
                        {
                            if (string.IsNullOrEmpty(specname)) //赋首个车型名称
                            {
                                specname = item.Name;
                                specid = item.Id;
                            }
                            if (item != null && item.Id == Convert.ToInt32(specID))
                            {
                                this.ispec.Text = item.Name;
                                isFind = true;
                                break;
                            }
                            else
                            { }
                        }
                        if (isFind)
                            break;

                    }
                    if (!isFind) //没有匹配的车型或车型id==0时，默认显示车系的第一款车型
                    {
                        this.ispec.Text = specname;
                        specID = specid.ToString();
                    }
                }
            }

            // this.askSubmit.IsEnabled = false;
        }

        private void InitSpecList()
        {

        }



        private void login_Click(object sender, EventArgs e)
        {
            var spec = specID;
            var name = this.iName.Text;
            var phone = this.iPhone.Text;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(phone))
            {
                string strPatern = @"(^1[3-8]\d{9}$|^\d{3}-\d{8}$|^\d{4}-\d{7}$)";
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(strPatern);
                if (!reg.IsMatch(this.iPhone.Text))
                {
                    Common.showMsg("手机号格式不正确");
                }
                else
                {
                    App.AskName = name;
                    App.AskPhone = phone;

                    object uniqueID = DeviceExtendedProperties.GetValue("DeviceUniqueId");
                    byte[] bID = (byte[])uniqueID;
                    String deviceID = Convert.ToBase64String(bID);
                    string data = string.Empty;
                    data += "a=2&pm=3&v=1.4.0&cityid=" + cityID + "&dealerid=" + dealerid + "&seriesid=" + seriesID + "&specid=" + specID + "&username=" + App.AskName + "&phone=" + App.AskPhone;
                    data += "&deviceid=" + deviceID + "&devicetoken=0" + "&siteid=29" + "&channelid=" + App.ChannelId;

                    WebClient wc = new WebClient();
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string url = string.Format("{0}{1}/dealer/AskPrice.ashx", App.appUrl, App.versionStr);
                    wc.UploadStringAsync(new Uri(url), "POST", data);
                    wc.UploadStringCompleted += new UploadStringCompletedEventHandler((ss, ee) =>
                    {

                        try
                        {

                            if (ee.Error != null)
                            {
                                Common.showMsg("提交失败");
                            }
                            else
                            {

                                this.NavigationService.GoBack();
                                Common.showMsg("提交成功");
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.showMsg("提交失败");
                        }


                    });


                }
            }
            else if (string.IsNullOrEmpty(name))
            {

                Common.showMsg("请输入姓名");


            }
            else if (string.IsNullOrEmpty(phone))
            {
                Common.showMsg("请输入手机号");
            }
        }
        private void iName_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(this.iName.Text) && !string.IsNullOrEmpty(this.iPhone.Text))
            //{
            //    this.askSubmit.IsEnabled = true;
            //}
        }
        private void iPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.iName.Text) && !string.IsNullOrEmpty(this.iPhone.Text))
            {
                string strPatern = @"(^1[3-8]\d{9}$|^\d{3}-\d{8}$|^\d{4}-\d{7}$)";
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(strPatern);
                if (!reg.IsMatch(this.iPhone.Text))
                {
                    //this.askSubmit.IsEnabled = true;
                    Common.showMsg("手机号格式不正确");
                }
            }

        }

        CarSeriesQuoteViewModel carSeriesQuoteVM = null;
        SpecSource specSource = new SpecSource();
        private class SpecGroup : List<CarSeriesQuoteViewModel>
        {


            public string key { get; set; }
            public string value { get; set; }
        }
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

            if (carSeriesQuoteVM == null)
            {
                carSeriesQuoteVM = new CarSeriesQuoteViewModel();
            }
            string url = string.Format("{0}{2}/cars/specslist-a2-pm3-v1.4.0-t0x000c-ss{1}.html", App.appUrl, seriesID, App.versionStr);
            carSeriesQuoteVM.LoadDataAysnc(url);

            carSeriesQuoteVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>>>(CarSeriesQuote_LoadDataCompleted);

        }
        void CarSeriesQuote_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error == null)
            {
                if (specID != "0")
                {
                    var iCar = from car in e.Result
                               where car.Id == int.Parse(specID)
                               select car;
                    var nam = iCar.FirstOrDefault();
                    this.ispec.Text = nam.Name;
                }
                else if (specID == "0")
                {
                    var nam = e.Result.ToList().FirstOrDefault();
                    this.ispec.Text = nam.Name;
                }
            }

        }

        private void ispec_GotFocus(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Car/ChooseSpec.xaml?dealerid=" + dealerid + "&cityID=" + cityID + "&seriesID=" + seriesID + "&specID=" + specID, UriKind.Relative));

        }





    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace AutoWP7.View.Car
{
    public partial class CarComparePage : PhoneApplicationPage
    {
        public CarComparePage()
        {
            InitializeComponent();
        }
        public CarCompareInfoViewModel carComPareInfoVM;
        public CarCompareInfoViewModels ccivms;
        public CarCompareViewModel carCompareViewModel;
        public List<int> specids;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            carComPareInfoVM = new CarCompareInfoViewModel();
            InitData();
        }

        private void InitData()
        {
            //要添加代码：取本地选中的对比车型的ID Start
            carCompareViewModel = new CarCompareViewModel();
            ccivms = new CarCompareInfoViewModels();

            //先隐藏删除按钮，等列表load完成后在显示
            this.delBtn1.Visibility = System.Windows.Visibility.Collapsed;
            this.delBtn2.Visibility = System.Windows.Visibility.Collapsed;

            var list = carCompareViewModel.GetCompareSpec();
            string specIdStr = string.Empty;
            //获取本地对比表里要对比的车型
            specids = new List<int>();
            foreach (var item in list)
                if (specids.Count >= 2)
                    break;

                else if (item.IsChoosed)
                {
                    specids.Add(item.SpecId);
                    specIdStr += item.SpecId + ",";
                    if (specNameTB1.Text == "")
                        specNameTB1.Text = item.SpecName;
                    else
                        specNameTB2.Text = item.SpecName;
                    if (seriesNameTB1.Text == "")
                        seriesNameTB1.Text = item.SeriesName;
                    else
                        seriesNameTB2.Text = item.SeriesName;
                }
            if (specIdStr.Trim().Length > 0)
                specIdStr = specIdStr.Remove(specIdStr.Length - 1);
            if (specids.Count > 0)
            {
                proptEmptyTB.Visibility = System.Windows.Visibility.Collapsed;
                spceNameGrid1.Visibility = System.Windows.Visibility.Visible;
                addCarCompareImg1.Visibility = System.Windows.Visibility.Collapsed;
                if (specids.Count >= 2)
                {
                    spceNameGrid2.Visibility = System.Windows.Visibility.Visible;
                    addCarCompareImg2.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                proptEmptyTB.Visibility = System.Windows.Visibility.Visible;
                spceNameGrid1.Visibility = System.Windows.Visibility.Collapsed;
                spceNameGrid2.Visibility = System.Windows.Visibility.Collapsed;
                addCarCompareImg1.Visibility = System.Windows.Visibility.Visible;
                addCarCompareImg2.Visibility = System.Windows.Visibility.Visible;
                carCompareInfoGroups.Visibility = System.Windows.Visibility.Collapsed;
            }
            //没有选中的对比车型
            if (!string.IsNullOrEmpty(specIdStr))
                CarSeriesQuoteLoadData(specIdStr);
        }

        private void delBtn1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool isEmpty = true;
            if (ccivms != null)
            {
                foreach (CarCompareInfoViewModel item in ccivms)
                {
                    foreach (ItemInfo info in item)
                    {
                        info.ItemValue1 = "";
                        if (!string.IsNullOrEmpty(info.ItemValue2))
                            isEmpty = false;
                    }
                }
                //更新本地的数据的选中状态代码Start
                if (specids.Count > 0)
                    carCompareViewModel.UpdateCompareSpec(specids[0], false);
                //更新本地的数据的选中状态代码Start

                addCarCompareImg1.Visibility = System.Windows.Visibility.Visible;
                spceNameGrid1.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            { }
            if (isEmpty)
            {
                carCompareInfoGroups.Visibility = System.Windows.Visibility.Collapsed;
                proptEmptyTB.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void delBtn2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool isEmpty = true;
            if (ccivms != null)
            {
                foreach (CarCompareInfoViewModel item in ccivms)
                {
                    foreach (ItemInfo info in item)
                    {
                        info.ItemValue2 = "";
                        if (!string.IsNullOrEmpty(info.ItemValue1))
                            isEmpty = false;
                    }
                }

                //更新本地的数据的选中状态代码Start
                if (specids.Count > 1)
                    carCompareViewModel.UpdateCompareSpec(specids[1], false);
                //更新本地的数据的选中状态代码Start
                carCompareInfoGroups.ItemsSource = ccivms;
                addCarCompareImg2.Visibility = System.Windows.Visibility.Visible;
                spceNameGrid2.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            { }
            if (isEmpty)
            {
                carCompareInfoGroups.Visibility = System.Windows.Visibility.Collapsed;
                proptEmptyTB.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public bool isHideSameItem = false;
        private void hideSameItem_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton abbtn = (ApplicationBarIconButton)sender;
            CarCompareInfoViewModels ccivs1 = new CarCompareInfoViewModels();
            CarCompareInfoViewModel i;
            if (!isHideSameItem)
            {
                isHideSameItem = true;
                foreach (CarCompareInfoViewModel item in ccivms)
                {
                    i = new CarCompareInfoViewModel(item.GroupName);
                    ItemInfo ii;
                    foreach (ItemInfo info in item)
                    {
                        if (info.ItemValue1 != info.ItemValue2)
                        {
                            ii = new ItemInfo()
                            {
                                ItemName = info.ItemName,
                                ItemValue1 = info.ItemValue1,
                                ItemValue2 = info.ItemValue2
                            };
                            i.Add(ii);
                        }
                    }
                    if (i.Count > 0)
                    {
                        ccivs1.Add(i);
                    }
                }
                carCompareInfoGroups.ItemsSource = ccivs1;
                abbtn.Text = "显示全部项";
                abbtn.IconUri = new Uri("/Images/bar_xshquanbu.png", UriKind.Relative);

            }
            else
            {
                isHideSameItem = false;
                carCompareInfoGroups.ItemsSource = ccivms;
                abbtn.Text = "隐藏相同项";
                abbtn.IconUri = new Uri("/Images/car_yincang.png", UriKind.Relative);
            }
        }

        private void addCarCompareImg1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Car/CarCompareListPage.xaml?action=2", UriKind.Relative));
        }

        private void addCarCompareImg2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Car/CarCompareListPage.xaml?action=2", UriKind.Relative));
        }

        /// <summary>
        /// 车系报价
        /// </summary>
        public void CarSeriesQuoteLoadData(string specIds)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            //清除表中以前的数据
            //using (LocalDataContext ldc = new LocalDataContext())
            //{
            //    var item = from s in ldc.carQuotes where s.Id > 0 select s;
            //    ldc.carQuotes.DeleteAllOnSubmit(item);
            //    ldc.SubmitChanges();
            //}

            //if (carComPareInfoVM == null)
            //{
            //    carComPareInfoVM = new CarCompareInfoViewModel();
            //}
            string url = string.Format("{0}{2}/cars/speccompare-a2-pm3-v1.6.2-t1-s{1}.html", App.appUrl, specIds, App.versionStr);
            carComPareInfoVM.LoadDataAysnc(url, specids);
            carComPareInfoVM.LoadDataCompleted += carComPareInfoVM_LoadDataCompleted;

        }

        void carComPareInfoVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarCompareInfoViewModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() < 1)
                {
                }
                else
                {
                    foreach (var entity in e.Result)
                    {
                        entity.Remove(entity.Find(s => s.ItemName == "车型名称"));
                        ItemInfo item = entity.FirstOrDefault(s => s.ItemName == "级别");
                        if (item != null)
                        {
                            structNameTB1.Text = item.ItemValue1;
                            structNameTB2.Text = item.ItemValue2;
                        }
                        entity.Remove(item);
                        //List<ItemInfo> items = entity.Where(w => w.ItemName == "级别" || w.ItemName == "车型名称") as List<ItemInfo>;

                        //foreach (var item in entity)
                        //{
                        //    bool isDel = false;
                        //    switch (item.ItemName)
                        //    {
                        //        case "级别":
                        //            isDel = true;
                        //            break;
                        //        case "车型名称":
                        //            isDel = true;
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //    if (isDel)
                        //        entity.Remove(item);
                        //}
                        ccivms.Add(entity);
                    }
                    carCompareInfoGroups.ItemsSource = ccivms;
                }
            }
        }
        /// <summary>
        /// 列表加载完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carCompareInfoGroups_Loaded(object sender, RoutedEventArgs e)
        {
            if (specids.Count == 1)
                delBtn1.Visibility = System.Windows.Visibility.Visible;
            if (specids.Count == 2)
            {
                delBtn1.Visibility = System.Windows.Visibility.Visible;
                delBtn2.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }

    public class CarCompareInfoViewModels : List<CarCompareInfoViewModel>
    {

    }
}
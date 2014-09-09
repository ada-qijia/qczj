using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using ViewModels;

namespace AutoWP7.View.Car
{
    public partial class CarCompareListPage : PhoneApplicationPage
    {
        public CarCompareListPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 添加对比按钮
        /// </summary>
        public static ApplicationBarIconButton AddVSBtn { get; set; }

        /// <summary>
        /// 对比按钮
        /// </summary>
        public static ApplicationBarIconButton ToVSBtn { get; set; }
        /// <summary>
        /// 全选按钮
        /// </summary>
        public static ApplicationBarIconButton SelectAlllBtn { get; set; }
        /// <summary>
        /// 编辑按钮
        /// </summary>
        public static ApplicationBarIconButton EditBtn { get; set; }
        /// <summary>
        /// 删除按钮
        /// </summary>
        public ApplicationBarIconButton DelBtn { get; set; }
        /// <summary>
        /// 当前操作状态：0,选择车型进行对比；1,编辑状态下，选择对比车型;2,重新选择对比车型
        /// </summary>
        public UseTypeEnum Action = 0;
        public int ALLCount = 0;
        public int SelectedCount = 0;
        public string SpecIds = string.Empty;
        public CarCompareViewModel carCompareViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SelectedCount = 0;
            Action = UseTypeEnum.Default;
            if (NavigationContext.QueryString.ContainsKey("action"))
            {
                Action = (UseTypeEnum)Convert.ToInt16(NavigationContext.QueryString["action"]);
            }

            carCompareViewModel = new CarCompareViewModel();
            switch (e.NavigationMode)
            {
                case NavigationMode.New:
                    break;
                case NavigationMode.Back:
                    Action = UseTypeEnum.Default;
                    break;
            }
            InitBtns();
            InitData();
        }

        #region 初始化按钮
        /// <summary>
        /// 初始化按钮
        /// </summary>
        private void InitBtns()
        {
            AddVSBtn = new ApplicationBarIconButton()
            {
                Text = "添加",
                IconUri = new Uri("/Images/car_addvs.png", UriKind.Relative)
            };
            AddVSBtn.Click += AddVSBtn_Click;
            ToVSBtn = new ApplicationBarIconButton()
            {
                Text = "对比",
                IconUri = new Uri("/Images/vs1.png", UriKind.Relative)
            };
            ToVSBtn.Click += ToVSBtn_Click;
            SelectAlllBtn = new ApplicationBarIconButton()
            {
                Text = "全选",
                IconUri = new Uri("/Images/selectall.png", UriKind.Relative)
            };
            SelectAlllBtn.Click += SelectAlllBtn_Click;
            EditBtn = new ApplicationBarIconButton()
            {
                Text = "编辑",
                IconUri = new Uri("/Images/bar_writeComment.png", UriKind.Relative)
            };
            EditBtn.Click += EditBtn_Click;
            DelBtn = new ApplicationBarIconButton()
            {
                Text = "删除",
                IconUri = new Uri("/Images/car_del.png", UriKind.Relative)
            };
            DelBtn.Click += DelBtn_Click;
        }

        #region 按钮事件
        /// <summary>
        /// 删除按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DelBtn_Click(object sender, EventArgs e)
        {
            List<int> specids = new List<int>();
            for (int i = 0; i < this.carCompareListBox.Items.Count; i++)
            {
                ListBoxItem item = this.carCompareListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (item != null)
                {
                    Grid gridItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<Grid>(item);
                    if (gridItem != null)
                    {
                        CheckBox checkBoxItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<CheckBox>(gridItem);
                        if (checkBoxItem != null && checkBoxItem.IsChecked == true)
                            specids.Add(Convert.ToInt32(gridItem.Tag));
                        else
                        { }
                    }
                    else
                    { }
                }
                else
                { }
            }
            //删除当前选中的车型
            carCompareViewModel.DeleteCompareSpec(specids);
            this.Action = UseTypeEnum.Default;
            InitData();
        }
        /// <summary>
        /// 全选按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectAlllBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.carCompareListBox.Items.Count; i++)
            {
                ListBoxItem item = this.carCompareListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (item != null)
                {
                    Grid gridItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<Grid>(item);
                    if (gridItem != null)
                    {
                        CheckBox checkBoxItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<CheckBox>(gridItem);
                        if (checkBoxItem != null)
                            checkBoxItem.IsChecked = true;
                        else
                        { }
                    }
                    else
                    { }
                }
                else
                { }
            }
            numTB.Text = "选中" + ALLCount + "/" + ALLCount;
        }

        void ToVSBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("对比");
        }
        void AddVSBtn_Click(object sender, EventArgs e)
        {
            //默认进入对比列表 -> 点"添加对比车型"
            //此情况在选完对比车型后，应跳转回到对比车型列表
            int state = 1;
            if (Action == UseTypeEnum.Default)
                state = 1;
            //从对比最终页添加车型 -> 对比车型列表 ->点"添加对比车型"
            //此情况在选完对比车型后，应跳转回到对比最终页
            else if (Action == UseTypeEnum.CompareSelect)
                state = 2;
            else
            { };
            this.NavigationService.Navigate(new Uri("/View/Car/CarChoosePage.xaml?action=" + state, UriKind.Relative));
        }
        void EditBtn_Click(object sender, EventArgs e)
        {
            Action = UseTypeEnum.Edit;
            //置所有车型均可选状态
            SetCheckBoxAndTextBlockState(true);
            //更换底部的按钮
            this.ApplicationBar.Buttons.Clear();
            this.ApplicationBar.Buttons.Add(SelectAlllBtn);
            this.ApplicationBar.Buttons.Add(DelBtn);
            startCompare.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion
        #endregion

        #region 页面数据初始化
        /// <summary>
        /// 
        /// </summary>
        private void InitData()
        {
            this.ApplicationBar.Buttons.Clear();
            SelectedCount = 0;

            ObservableCollection<CarCompareViewModel> dc = new ObservableCollection<CarCompareViewModel>();

            if (Action == UseTypeEnum.Default)
            {
                dc = carCompareViewModel.GetCompareSpec();
                int count = 0;
                foreach (var item in dc)
                {
                    if (item.IsChoosed == true)
                    {
                        if (count >= 2)
                        {
                            item.IsChoosed = false;
                            carCompareViewModel.UpdateCompareSpec(item.SpecId, false);
                        }
                        else
                            count++;
                    }
                    else
                    { }
                }
            }
            else if (Action == UseTypeEnum.CompareSelect)
            {
                dc = carCompareViewModel.GetCompareSpec(2);
            }
            else if (Action == UseTypeEnum.Edit)
            {
                dc = carCompareViewModel.GetCompareSpec();
            }
            foreach (var item in dc)
            {
                item.SpecName = item.SeriesName + " " + item.SpecName;
                if (item.IsChoosed)
                    SelectedCount++;
            }
            ALLCount = dc.Count;
            numTB.Text = "已选" + SelectedCount + "/" + ALLCount;
            carCompareListBox.ItemsSource = dc;

            if (Action == UseTypeEnum.Default)
            {
                if (ALLCount > 0)
                    this.ApplicationBar.Buttons.Add(EditBtn);
                if (ALLCount <= 9)
                    this.ApplicationBar.Buttons.Add(AddVSBtn);

                if (SelectedCount >= 2) //只有选中两个车型了，才显示对比按钮
                    startCompare.Background = new SolidColorBrush(Color.FromArgb(0xff, 0060, 0xac, 0xeb));
                else
                    startCompare.Background = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
            }
            else if (Action == UseTypeEnum.CompareSelect)
            {
                numTB.Visibility = System.Windows.Visibility.Collapsed;
                startCompare.Visibility = System.Windows.Visibility.Collapsed;
                this.ApplicationBar.Buttons.Add(AddVSBtn);
            }
            else if (Action == UseTypeEnum.Edit)
            {
                this.ApplicationBar.Buttons.Add(SelectAlllBtn);
                this.ApplicationBar.Buttons.Add(DelBtn);
            }
            else
            { }

            if (ALLCount <= 0)
            {
                promptGrid.Visibility = Visibility.Visible;
                carCompareListBox.Visibility = System.Windows.Visibility.Collapsed;
                startCompare.Visibility = System.Windows.Visibility.Collapsed;
                numTB.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                promptGrid.Visibility = Visibility.Collapsed;
                carCompareListBox.Visibility = System.Windows.Visibility.Visible;
                startCompare.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion
        /// <summary>
        /// ListBox加载完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void carCompareListBox_Loaded(object sender, RoutedEventArgs e)
        {
            //目前load只有两种情况：
            //1、点连接或按钮进入对比列表（Action=UseTypeEnum.Default）
            //2、对比最终页重选车系（Action == UseTypeEnum.CompareSelect）
            if (Action == UseTypeEnum.Default)
            {
                //最多只能同时对比两个，其他置为不可用状态
                if (SelectedCount >= 2)
                {
                    SetCheckBoxAndTextBlockState(false);
                    startCompare.Background = new SolidColorBrush(Color.FromArgb(0xff, 0060, 0xac, 0xeb));
                }
                else
                {
                    startCompare.Background = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
                }
                numTB.Text = "已选" + SelectedCount + "/" + ALLCount;
            }
            else if (Action == UseTypeEnum.CompareSelect) //隐藏复选框和选中数量提示框
            {
                numTB.Visibility = System.Windows.Visibility.Collapsed;
                startCompare.Visibility = System.Windows.Visibility.Collapsed;
                for (int i = 0; i < this.carCompareListBox.Items.Count; i++)
                {
                    ListBoxItem item = this.carCompareListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        Grid gridItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<Grid>(item);
                        if (gridItem != null)
                        {
                            CheckBox checkBoxItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<CheckBox>(gridItem);
                            if (checkBoxItem != null)
                            {
                                checkBoxItem.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            else
                            { }
                        }
                        else
                        { }
                    }
                    else
                    { }
                }
            }
            else
            { }
        }
        bool isbuy = false;
        private void carComPareCB_Click(object sender, RoutedEventArgs e)
        {
            if (isbuy)
            { }
            else
            {
                isbuy = true;
                CheckBox cb = (CheckBox)sender;
                Grid gg = (Grid)cb.Parent;
                string specid = gg.Tag.ToString();
                //对比车型条目点击事件
                if (Action == UseTypeEnum.CompareSelect)
                {
                    //更改本地对比表对应车型的选中状态（此时此车型IsChoosed=false）
                    carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), true);
                    //跳转到对比最终页
                    this.NavigationService.Navigate(new Uri("/View/Car/CarComparePage.xaml", UriKind.Relative));
                }
                else if (Action == UseTypeEnum.Default)
                {
                    if (cb.IsChecked == true)//之前是选中状态，现在要取消选中
                    {
                        if (SelectedCount >= 2) //最多只能对比两个
                            cb.IsChecked = false;
                        else
                        {
                            SelectedCount += 1;
                            //更改选中状态
                            carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), true);
                            //更改所有车型条目未可选状态
                            if (SelectedCount >= 2)
                                SetCheckBoxAndTextBlockState(false);
                        }
                    }
                    else //之前是未选中状态，现在要置为选中状态
                    {
                        SelectedCount = SelectedCount > 0 ? SelectedCount -= 1 : 0;
                        //更改选中状态
                        carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), false);
                        //将未选中的置为不可用状态
                        SetCheckBoxAndTextBlockState(true);

                    }
                    if (SelectedCount >= 2) //只有选中两个车型了，才显示对比按钮
                        startCompare.Background = new SolidColorBrush(Color.FromArgb(0xff, 0060, 0xac, 0xeb));
                    else
                        startCompare.Background = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
                    //更改头部提示选中数量
                    numTB.Text = "已选" + SelectedCount + "/" + this.carCompareListBox.Items.Count;
                }
                else if (Action == UseTypeEnum.Edit)
                {
                    //当前选中状态
                    bool isChecked = (bool)cb.IsChecked;
                    //更改选中状态
                    //carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), !isChecked);
                    //更改头部提示选中数量
                    if (isChecked)
                        SelectedCount += 1;
                    else
                        SelectedCount -= 1;
                    numTB.Text = "已选" + SelectedCount + "/" + this.carCompareListBox.Items.Count;
                }
                else
                { }
                isbuy = false;
            }
        }
        private void carCompareNameTB_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isbuy)
            { }
            else
            {
                isbuy = true;
                TextBlock tb = (TextBlock)sender;
                Grid gg = (Grid)tb.Parent;
                string specid = gg.Tag.ToString();
                //对比车型条目点击事件
                if (Action == UseTypeEnum.CompareSelect)
                {
                    //更改本地对比表对应车型的选中状态（此时此车型IsChoosed=false）
                    carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), true);
                    //跳转到对比最终页
                    this.NavigationService.Navigate(new Uri("/View/Car/CarComparePage.xaml", UriKind.Relative));
                }
                else if (Action == UseTypeEnum.Default)
                {
                    CheckBox curCheckBox = CommonLayer.CommonHelper.FindFirstElementInVisualTree<CheckBox>(gg);
                    if (curCheckBox != null)
                    {
                        if (curCheckBox.IsChecked == true)//之前是选中状态，现在要取消选中
                        {
                            SelectedCount = SelectedCount >= 1 ? --SelectedCount : 0;
                            //更改CheckBox的选中状态
                            curCheckBox.IsChecked = false;
                            //更改选中状态
                            carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), false);
                            //更改所有车型条目未可选状态
                            SetCheckBoxAndTextBlockState(true);
                        }
                        else //之前是未选中状态，现在要置为选中状态
                        {
                            if (SelectedCount >= 2) //最多选中两个（只能同时对比两个）
                            { }
                            else
                            {
                                SelectedCount += 1;
                                //更改选中状态
                                carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), true);
                                //更改CheckBox的选中状态
                                curCheckBox.IsChecked = true;
                                //更改所有车型条目未可选状态
                                if (SelectedCount >= 2)
                                {
                                    //将未选中的置为不可用状态
                                    SetCheckBoxAndTextBlockState(false);
                                }
                            }
                        }
                        if (SelectedCount >= 2) //只有选中两个车型了，才显示对比按钮
                            startCompare.Background = new SolidColorBrush(Color.FromArgb(0xff, 0060, 0xac, 0xeb));
                        else
                            startCompare.Background = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
                    }
                    //更改头部提示选中数量
                    numTB.Text = "已选" + SelectedCount + "/" + this.carCompareListBox.Items.Count;
                }
                else if (Action == UseTypeEnum.Edit)
                {
                    CheckBox curCheckBox = CommonLayer.CommonHelper.FindFirstElementInVisualTree<CheckBox>(gg);
                    if (curCheckBox != null)
                    {
                        //当前选中状态
                        bool isChecked = (bool)curCheckBox.IsChecked;
                        //更改选中状态
                        //carCompareViewModel.UpdateCompareSpec(Convert.ToInt32(specid), !isChecked);
                        //更改CheckBox的选中状态
                        curCheckBox.IsChecked = !isChecked;
                        //更改头部提示选中数量
                        if (isChecked)
                            SelectedCount -= 1;
                        else
                            SelectedCount += 1;
                        numTB.Text = "已选" + SelectedCount + "/" + this.carCompareListBox.Items.Count;
                    }
                }
                else
                { }
                isbuy = false;
            }
        }


        /// <summary>
        /// 设置CheckBox和TextBox不可用
        /// </summary>
        /// <param name="cb"></param>
        private void SetCheckBoxUnEnable(CheckBox cb, TextBlock tb)
        {
            if (cb != null)
            {
                cb.IsHitTestVisible = false;
                cb.Foreground = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
                cb.Background = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
                cb.BorderBrush = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
            }
            if (tb != null)
            {
                tb.Foreground = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
            }
        }
        /// <summary>
        /// 设置CheckBox和TextBox可用
        /// </summary>
        /// <param name="cb"></param>
        private void SetCheckBoxEnable(CheckBox cb, TextBlock tb)
        {
            if (cb != null)
            {
                cb.IsHitTestVisible = true;
                cb.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0x00, 0x00));
                cb.Background = new SolidColorBrush(Color.FromArgb(0060, 0060, 0060, 0060));
                cb.BorderBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x3c, 0xad, 0xeb));
            }
            if (tb != null)
            {
                tb.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0x00, 0x00));
            }
        }

        private void startCompare_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (SelectedCount != 2)
                MessageBox.Show("抱歉，只能同时对比2个车型");
            else
                this.NavigationService.Navigate(new Uri("/View/Car/CarComparePage.xaml", UriKind.Relative));
        }
        /// <summary>
        /// 设定复选框可车型名称是否可用样式
        /// </summary>
        /// <param name="IsEnable"></param>
        private void SetCheckBoxAndTextBlockState(bool IsEnable)
        {
            if (this.carCompareListBox != null && this.carCompareListBox.Items != null && this.carCompareListBox.Items.Count > 0)
            {
                for (int i = 0; i < this.carCompareListBox.Items.Count; i++)
                {
                    ListBoxItem item = this.carCompareListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        Grid gridItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<Grid>(item);
                        if (gridItem != null)
                        {
                            CheckBox checkBoxItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<CheckBox>(gridItem);
                            TextBlock textBlockItem = CommonLayer.CommonHelper.FindFirstElementInVisualTree<TextBlock>(item);
                            if (checkBoxItem != null && textBlockItem != null)
                            {
                                if (IsEnable) //置为可被选中状态
                                    SetCheckBoxEnable(checkBoxItem, textBlockItem);
                                else if (checkBoxItem.IsChecked == false) //将未选中的条目置为不可选状态
                                    SetCheckBoxUnEnable(checkBoxItem, textBlockItem);
                                else
                                { }
                            }
                            else
                            { }
                        }
                        else
                        { }
                    }
                    else
                    { }
                }
            }
            else
            { }
        }
    }
    public class CarCompareModels : List<CarCompareViewModel>
    {
    }
    /// <summary>
    /// 当前列表用途
    /// </summary>
    public enum UseTypeEnum
    {
        /// <summary>
        /// 默认选择对比车型
        /// </summary>
        Default = 0,
        /// <summary>
        /// 编辑对比车型
        /// </summary>
        Edit = 1,
        /// <summary>
        /// 从对比最终页跳转过来选择对比车型
        /// </summary>
        CompareSelect = 2
    }
}
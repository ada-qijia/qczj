using Microsoft.Phone.Controls;
using System;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class OthersHomePage : PhoneApplicationPage
    {
        private OthersViewModel OthersVM;

        private string userID;

        public OthersHomePage()
        {
            InitializeComponent();

            this.OthersVM = new OthersViewModel();
            this.OthersVM.LoadDataCompleted += ReplyVM_LoadDataCompleted;
            this.DataContext = this.OthersVM;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                userID = this.NavigationContext.QueryString["userID"].ToString();
                this.LoadMore(true);
            }
        }

        #region load data

        private void ReplyVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
        }

        private void LoadMore(bool restart)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            int nextPageIndex = this.OthersVM.PageIndex + 1;
            if (restart)
            {
                this.OthersVM.ClearData();
                nextPageIndex = 1;
            }

            string url = Utils.MeHelper.GetUserInfoUrl(userID, nextPageIndex);
            this.OthersVM.LoadDataAysnc(url);
        }

        #endregion

        #region UI interaction

        //不支持导航
        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }


        #endregion
    }
}
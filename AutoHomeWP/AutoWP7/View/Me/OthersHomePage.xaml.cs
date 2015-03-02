using Microsoft.Phone.Controls;
using Model.Me;
using System;
using System.Windows;
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

        //导航到帖子详情页
        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TritanModel model = (sender as FrameworkElement).DataContext as TritanModel;
            if (model != null)
            {
                View.Forum.TopicDetailPage.ShareTitle(model.Title);
                string url = string.Format("/View/Forum/TopicDetailPage.xaml?from=0&bbsId={0}&topicId={1}&bbsType={2}", model.BBSID, model.ID, model.BBSType);
                this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }


        #endregion
    }
}
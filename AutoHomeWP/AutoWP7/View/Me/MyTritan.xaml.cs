using Microsoft.Phone.Controls;
using Model.Me;
using System;
using System.Windows;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class MyTritan : PhoneApplicationPage
    {
        private TritanViewModel TritanVM;

        public MyTritan()
        {
            InitializeComponent();

            this.TritanVM = new TritanViewModel();
            this.TritanVM.LoadDataCompleted += TritanVM_LoadDataCompleted;
            this.DataContext = this.TritanVM;
            this.LoadMore(true);
        }

        private void TritanVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            bool noResult = this.TritanVM.RowCount == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent("您还没有发表帖子哦~");
            }
            this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
            this.TopicListBox.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LoadMore(bool restart)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            int nextPageIndex = this.TritanVM.PageIndex + 1;
            if (restart)
            {
                this.TritanVM.ClearData();
                nextPageIndex = 1;
            }

            string url = Utils.MeHelper.GetMyTritanUrl(nextPageIndex);
            this.TritanVM.LoadDataAysnc(url);
        }

        #region UI interaction

        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TritanModel model = (sender as FrameworkElement).DataContext as TritanModel;
            if (model != null)
            {
                View.Forum.TopicDetailPage.ShareTitle(model.Title);
                string url = string.Format("/View/Forum/TopicDetailPage.xaml?from=0&bbsId={0}&topicId={1}&bbsType={2}", model.BBSID, model.ID, model.BBSType);
                this.NavigationService.Navigate(new Uri(url,UriKind.Relative));
            }
        }

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }

        #endregion
    }
}
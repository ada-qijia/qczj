using AutoWP7.Utils;
using Model.Me;
using System;
using System.Collections;
using System.Windows;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class DraftBox : MultiSelectablePage
    {
        private DraftViewModel DraftVM;

        public DraftBox()
        {
            InitializeComponent();

            //multiSelectable setting
            this.CurrentList = this.DraftList;

            this.DraftVM = DraftViewModel.SingleInstance;
            this.DataContext = this.DraftVM;

            foreach (var item in this.DraftVM.DraftList)
            {
                item.read = true;
            }

            this.DraftList_CollectionChanged(this, null);
            this.DraftVM.DraftList.CollectionChanged += DraftList_CollectionChanged;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DraftVM.UnReadCount = 0;
        }

        private void DraftList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool noResult = this.DraftVM.DraftList.Count == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent("暂无草稿");
            }
            this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
            this.DraftList.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Draft_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as DraftModel;
            if (model != null)
            {
                //导航到发帖页面
                if (string.IsNullOrEmpty(model.TopicID))
                {
                    string url = string.Format("/View/Forum/SendLetterPage.xaml?title={0}&bbsId={1}&bbsType={2}&savedTime={3}", "发帖", model.BBSID, model.BBSType, model.SavedTime.ToString());
                    this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
                }
                //导航到回帖页面
                else
                {
                    string url = string.Format("/View/Forum/ReplyCommentPage.xaml?bbsId={0}&bbsType={1}&targetReplyId={2}&topicId={3}&url=creatReply&pageindex=&title={4}&savedTime={5}", model.BBSID, model.BBSType, model.TargetReplyID, model.TopicID, model.Title, model.SavedTime.ToString());
                    this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
                }
            }
        }

        #region override MultiSelectablePage method

        public override void AfterDeleteItems(IList selectedItems)
        {
            base.AfterDeleteItems(selectedItems);

            this.DraftVM.SaveDraft();
        }

        #endregion
    }
}
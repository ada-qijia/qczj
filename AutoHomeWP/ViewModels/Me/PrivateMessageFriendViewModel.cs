using CommonLayer;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Me
{
    public class PrivateMessageFriendViewModel : Search.SearchResultViewModelBase
    {
        public PrivateMessageFriendViewModel()
        {
            this.FriendList = new ObservableCollection<PrivateMessageFriendModel>();

            this.LoadMoreButtonItem = new PrivateMessageFriendModel() { IsLoadMore = true };
            this.DownloadStringCompleted += ViewModel_DownloadStringCompleted;
        }

        #region properties

        public ObservableCollection<PrivateMessageFriendModel> FriendList { get; private set; }

        private int _returnCode;
        public int ReturnCode
        {
            get { return _returnCode; }
            set { SetProperty<int>(ref _returnCode, value); }
        }

        #endregion

        #region interface implementation

        public event EventHandler LoadDataCompleted;

        private bool isLoading = false;
        public void LoadDataAysnc(string url)
        {
            if (!isLoading)
            {
                isLoading = true;

                //开始下载
                this.DownloadStringAsync(url);
            }
        }

        private void ViewModel_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                try
                {
                    this.TryRemoveMoreButton();

                    //返回的json数据
                    JObject json = JObject.Parse(e.Result);
                    JToken resultToken = json.SelectToken("result");
                    this.ReturnCode = json.SelectToken("returncode").Value<int>();

                    #region 用返回结果填充每个版块

                    this.RowCount = resultToken.SelectToken("rowcount").Value<int>();
                    this.PageIndex = resultToken.SelectToken("pageindex").Value<int>();
                    this.PageCount = resultToken.SelectToken("pagecount").Value<int>();

                    JToken blockToken;

                    //用户列表
                    if(this.PageIndex==1)
                    {
                        this.FriendList.Clear();
                    }

                    blockToken = resultToken.SelectToken("list");
                    if (blockToken.HasValues)
                    {
                        var friendsList = JsonHelper.DeserializeOrDefault<List<PrivateMessageFriendModel>>(blockToken.ToString());
                        if (friendsList != null)
                        {
                            foreach (var model in friendsList)
                            {
                                this.FriendList.Add(model);
                            }
                        }
                    }

                    #endregion

                    this.EnsureMoreButton();
                }
                catch
                {
                }
            }

            isLoading = false;

            //触发完成事件
            if (LoadDataCompleted != null)
            {
                LoadDataCompleted(this, null);
            }
        }

        public void ClearData()
        {
            this.RowCount = 0;
            this.PageIndex = 0;
            this.PageCount = 0;

            this.FriendList.Clear();
        }

        #endregion

        #region base class override
        protected override void EnsureMoreButton()
        {
            if (!this.IsEndPage && !this.FriendList.Contains(this.LoadMoreButtonItem))
            {
                this.FriendList.Add((PrivateMessageFriendModel)this.LoadMoreButtonItem);
            }
        }

        protected override void TryRemoveMoreButton()
        {
            if (this.FriendList.Contains(this.LoadMoreButtonItem))
            {
                this.FriendList.Remove((PrivateMessageFriendModel)this.LoadMoreButtonItem);
            }
        }
        #endregion
    }

}

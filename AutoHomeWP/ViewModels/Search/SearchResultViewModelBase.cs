using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Search
{
    public abstract class SearchResultViewModelBase : Model.BindableBase
    {
        protected event DownloadStringCompletedEventHandler DownloadStringCompleted;

        private int _rowCount;
        public int RowCount
        {
            get { return _rowCount; }
            set
            { SetProperty<int>(ref _rowCount, value); }
        }

        private string _keyword;
        public string Keyword
        {
            get { return _keyword; }
            set
            {
                SetProperty<string>(ref _keyword, value);
            }
        }

        #region 多页分批加载

        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public bool IsEndPage
        {
            get { return PageIndex >= PageCount; }
        }

        public LoadMoreItem LoadMoreButtonItem { get; set; }

        /// <summary>
        /// Nothing was done here. Override it with your custom logic.
        /// </summary>
        protected virtual void EnsureMoreButton()
        { }

        /// <summary>
        /// Nothing was done here. Override it with your custom logic.
        /// </summary>
        protected virtual void TryRemoveMoreButton()
        { }

        #endregion

        protected void DownloadStringAsync(string url)
        {
            WebClient wc = new WebClient();

            wc.Encoding = System.Text.Encoding.UTF8;
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";

            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((obj, e) =>
            {
                if (this.DownloadStringCompleted != null)
                {
                    this.DownloadStringCompleted(this, e);
                }
            });

            Uri urlSource = new Uri(url + "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
        }
    }
}

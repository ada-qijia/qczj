using Model.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using ViewModels.Handler;
using Newtonsoft.Json.Linq;
using CommonLayer;

namespace ViewModels.Search
{
    public class VideoSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public VideoSearchResultViewModel()
        {
            this.VideoList = new ObservableCollection<VideoSearchModel>();
        }

        #region properties

        public int VideoCount { get; set; }

        public ObservableCollection<VideoSearchModel> VideoList { get; private set; }

        #endregion

        #region interface implementation

        public event EventHandler LoadDataCompleted;

        private bool isLoading = false;
        public void LoadDataAysnc(string url)
        {
            if (!isLoading)
            {
                isLoading = true;

                this.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
                {
                    if (ee.Error == null && ee.Result != null)
                    {
                        try
                        {
                            //返回的json数据
                            JObject json = JObject.Parse(ee.Result);
                            JToken resultToken = json.SelectToken("result");

                            #region 用返回结果填充每个版块

                            this.VideoCount = resultToken.SelectToken("rowcount").Value<int>();

                            JToken blockToken;
                            //视频列表
                            blockToken = resultToken.SelectToken("list");
                            if (blockToken.HasValues)
                            {
                                var modelList = JsonHelper.DeserializeOrDefault<List<VideoSearchModel>>(blockToken.ToString());
                                if (modelList != null)
                                {
                                    foreach (var model in modelList)
                                    {
                                        this.VideoList.Add(model);
                                    }
                                }
                            }

                            #endregion
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
                });

                //开始下载
                this.DownloadStringAsync(url);
            }
        }

        public void ClearData()
        {
            this.VideoCount = 0;
            this.VideoList.Clear();
        }

        #endregion
    }
}

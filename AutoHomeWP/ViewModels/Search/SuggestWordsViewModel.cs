using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using ViewModels.Handler;

namespace ViewModels.Search
{
    public class SuggestWordsViewModel:SearchResultViewModelBase, ISearchViewModel
    {
        public SuggestWordsViewModel()
        {
            this.SuggestWordsList = new ObservableCollection<string>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        public ObservableCollection<string> SuggestWordsList
        { get; private set; }

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

                            this.RowCount = resultToken.SelectToken("rowcount").Value<int>();

                            //视频列表
                            this.SuggestWordsList.Clear();
                            JArray blockToken = (JArray)resultToken.SelectToken("wordlist");
                            if (blockToken != null)
                            {
                                for (int i = 0; i < blockToken.Count; i++)
                                {
                                    string word = blockToken[i].SelectToken("name").ToString();
                                    SuggestWordsList.Add(word);
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
            this.SuggestWordsList.Clear();
        }
    }
}

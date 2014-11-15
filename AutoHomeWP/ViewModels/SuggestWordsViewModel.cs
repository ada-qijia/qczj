using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using ViewModels.Handler;

namespace ViewModels
{
    public class SuggestWordsViewModel
    {
        public SuggestWordsViewModel()
        {
            WordsDataSource = new ObservableCollection<string>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        public ObservableCollection<string> WordsDataSource
        { get; private set; }

        public event EventHandler<APIEventArgs<IEnumerable<string>>> LoadDataCompleted;

        public void LoadDataAysnc(string url)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }
            
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";          
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<string>> apiArgs = new APIEventArgs<IEnumerable<string>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    try
                    {
                        //返回的json数据
                        JObject json = JObject.Parse(ee.Result);
                        JArray wordsJson = (JArray)json.SelectToken("result").SelectToken("wordlist");

                        NewsModel model = null;
                        for (int i = 0; i < wordsJson.Count; i++)
                        {
                            string word = wordsJson[i].SelectToken("name").ToString();
                            WordsDataSource.Add(word);
                        }
                    }
                    catch
                    {

                    }
                }

                Uri urlSource = new Uri(url + "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
                wc.DownloadStringAsync(urlSource);

                //返回结果集
                apiArgs.Result = WordsDataSource;
                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this,apiArgs);
                }
            });
        }
    }
}

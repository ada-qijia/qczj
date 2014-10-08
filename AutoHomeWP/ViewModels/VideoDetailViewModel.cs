using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Model;
using ViewModels.Handler;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ViewModels
{
    public class VideoDetailViewModel : BindableBase
    {
        public VideoDetailViewModel()
        {
        }

        private VideoDetailModel _dataSource;
        public VideoDetailModel DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (value != _dataSource)
                {
                    _dataSource = value;
                    OnPropertyChanged("DataSource");
                }
            }
        }

        public event EventHandler<APIEventArgs<VideoDetailModel>> LoadDataCompleted;

        WebClient wc = null;
        public void LoadDataAysnc(string url)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }
            if (wc.IsBusy == true)
            {
                wc.CancelAsync();
            }

            //wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<VideoDetailModel> apiArgs = new APIEventArgs<VideoDetailModel>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    if (json != null)
                    {
                        JToken result = json.SelectToken("result");

                        VideoDetailModel model = null;
                        if (result != null)
                        {
                            model = new VideoDetailModel();
                            model.Title = (string)result.SelectToken("title");
                            model.PicUrl = (string)result.SelectToken("picurl");
                            model.PlayTimes = (int)result.SelectToken("playtimes");
                            model.CommentNum = (int)result.SelectToken("commentnum");
                            model.InputTime = (string)result.SelectToken("inputtime");
                            model.CategoryName = (string)result.SelectToken("categoryname");
                            //model.Description = (string)result.SelectToken("description");
                            model.NickName = (string)result.SelectToken("nickName");
                            model.VideoAddress = (string)result.SelectToken("videoaddress");
                            model.YoukuVideoKey = (string)result.SelectToken("youkuvideokey");

                            //description
                            string descStr = (string)result.SelectToken("description");
                            descStr = descStr.Replace("<div>", "").Replace("<DIV>", "").Replace("<Div>", "").Replace("</div>", "").Replace("</DIV>", "").Replace("</Div>", "")
                            .Replace("&nbsp;", "")
                            .Replace("<br />", "").Replace("<br/>", "").Replace("<BR />", "").Replace("<BR/>", "").Replace("<Br />", "").Replace("<Br/>", "")
                            .Replace("<br>", "").Replace("<br >", "").Replace("<BR>", "").Replace("<BR >", "").Replace("<Br>", "").Replace("<Br >", "");
                            descStr = descStr.Trim(' ');
                            model.Description = descStr;

                            //relation list
                            var ja = (JArray)result.SelectToken("relationvideolist");
                            if (ja != null && ja.Count > 0)
                            {
                                var list = new List<RelationVideoModel>();
                                foreach (var item in ja)
                                {
                                    RelationVideoModel rv = new RelationVideoModel();
                                    rv.ID = (int)item.SelectToken("id");
                                    rv.Title = (string)item.SelectToken("title");
                                    rv.PicUrl = (string)item.SelectToken("picurl");
                                    rv.InputTime = (string)item.SelectToken("inputtime");
                                    rv.PlayTimes = (int)item.SelectToken("playtimes");
                                    list.Add(rv);
                                }
                                model.RelationVideoList = list;
                            }

                            //model.RelationVideoList = (IEnumerable<RelationVideoModel>)();
                            DataSource = model;
                        }
                    }
                }
                //注意
                apiArgs.Result = DataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }

            });
        }

    }
}

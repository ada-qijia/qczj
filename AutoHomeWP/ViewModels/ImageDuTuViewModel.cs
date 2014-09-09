using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;

namespace ViewModels
{
    public class ImageDuTuViewModel : BaseViewModel
    {
        public ImageDuTuViewModel()
        {
            _imageDataSource = new ObservableCollection<ImageDuTuModel>();
        }
        /// <summary>
        /// 最新评论集合
        /// </summary>
        public ObservableCollection<ImageDuTuModel> _imageDataSource;
        public ObservableCollection<ImageDuTuModel> ImageDataSource
        {
            get
            {
                return _imageDataSource;
            }
            set
            {
                if (value != _imageDataSource)
                {
                    OnPropertyChanging("ImageDataSource");
                    _imageDataSource = value;
                    OnPropertyChanged("ImageDataSource");
                }
            }
        }


        //事件通知
        public event EventHandler<APIEventArgs<IEnumerable<ImageDuTuModel>>> LoadDataCompleted;

        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        public void LoadDataAysnc(string url, int Type)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }
            wc.Encoding = new Gb2312Encoding();

            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<ImageDuTuModel>> apiArgs = new APIEventArgs<IEnumerable<ImageDuTuModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    //清空数据
                    ImageDataSource.Clear();

                    try
                    {

                        //返回的json数据
                        JObject json = JObject.Parse(ee.Result);
                        if (((int)json.SelectToken("result").SelectToken("rowcount")) > 0)
                        {
                            #region 获取数据
                            JArray imgJson = (JArray)json.SelectToken("result").SelectToken("list");

                            ImageDuTuModel model = null;
                            switch (Type)
                            {
                                case 1:
                                    for (int i = 0; i < imgJson.Count; i++)
                                    {
                                        model = new ImageDuTuModel();
                                        model.Index = i;
                                        model.Url = (string)imgJson[i].SelectToken("img"); ;
                                        ImageDataSource.Add(model);
                                    }
                                    break;
                                case 2:
                                    for (int i = 0; i < imgJson.Count; i++)
                                    {
                                        model = new ImageDuTuModel();
                                        model.Index = i;
                                        model.Url = (string)imgJson[i].SelectToken("bigpic");
                                        model.BigUrl = "";
                                        model.SmallUrl = "";
                                        ImageDataSource.Add(model);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    //注意
                    apiArgs.Result = ImageDataSource;

                }

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });

        }
    }
}

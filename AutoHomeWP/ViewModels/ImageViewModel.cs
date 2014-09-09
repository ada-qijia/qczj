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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ViewModels
{

    public class ImageViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        //属性更改时的事件
        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        //属性更改完毕事件
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ImageViewModel()
        {
            _imageDataSource = new ObservableCollection<ImageModel>();
        }

        /// <summary>
        /// 最新评论集合
        /// </summary>
        public ObservableCollection<ImageModel> _imageDataSource;
        public ObservableCollection<ImageModel> ImageDataSource
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
        public event EventHandler<APIEventArgs<IEnumerable<ImageModel>>> LoadDataCompleted;

        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        public void LoadDataAysnc(string url)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }
            wc.Encoding = new Gb2312Encoding();

            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url , UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<ImageModel>> apiArgs = new APIEventArgs<IEnumerable<ImageModel>>();
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
                            JArray imgJson = (JArray)json.SelectToken("result").SelectToken("piclist");

                            ImageModel model = null;

                            for (int i = 0; i < imgJson.Count; i = i + 3)
                            {
                                model = new ImageModel();
                                if (i < imgJson.Count)
                                {
                                    model.SmallPic = (string)imgJson[i].SelectToken("smallpic");
                                }

                                if ((i + 1) < imgJson.Count)
                                {
                                    model.SmallPicTwo = (string)imgJson[i + 1].SelectToken("smallpic");
                                }
                                if ((i + 2) < imgJson.Count)
                                {
                                    model.SmallPicThree = (string)imgJson[i + 2].SelectToken("smallpic");
                                }
                                model.SpecName = (string)imgJson[i].SelectToken("specname");
                                model.Total = (int)json.SelectToken("result").SelectToken("rowcount");
                                ImageDataSource.Add(model);

                            }
                            model.ID = (int)json.SelectToken("result").SelectToken("pageindex");
                            model = new ImageModel();
                            model.loadMore = "点击加载更多...";
                            model.Total = (int)json.SelectToken("result").SelectToken("rowcount");
                            ImageDataSource.Add(model);



                            //图片加载
                            for (int i = 0; i < ImageDataSource.Count - 1; i++)
                            {
                                if (!string.IsNullOrEmpty(ImageDataSource[i].SmallPic))
                                {
                                    ImageDataSource[i].bitmap = new StorageCachedImage(new Uri(ImageDataSource[i].SmallPic, UriKind.Absolute));
                                }
                            }

                            for (int j = 0; j < ImageDataSource.Count - 1; j++)
                            {
                                if (!string.IsNullOrEmpty(ImageDataSource[j].SmallPicTwo))
                                {
                                    ImageDataSource[j].bitmapTwo = new StorageCachedImage(new Uri(ImageDataSource[j].SmallPicTwo, UriKind.Absolute));
                                }
                            }

                            for (int k = 0; k < ImageDataSource.Count - 1; k++)
                            {
                                if (!string.IsNullOrEmpty(ImageDataSource[k].SmallPicThree))
                                {
                                    ImageDataSource[k].bitmapThree = new StorageCachedImage(new Uri(ImageDataSource[k].SmallPicThree, UriKind.Absolute));
                                }
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

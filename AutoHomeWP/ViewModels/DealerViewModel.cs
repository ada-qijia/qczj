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
    public class DealerViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public DealerViewModel()
        {
            _dealerDataSource = new ObservableCollection<DealerModel>();
        }
        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<DealerModel> _dealerDataSource;
        public ObservableCollection<DealerModel> DealerDataSource
        {
            get
            {
                return _dealerDataSource;
            }
            set
            {
                if (value != _dealerDataSource)
                {
                    OnPropertyChanging("DealerDataSource");
                    _dealerDataSource = value;
                    OnPropertyChanged("DealerDataSource");
                }
            }
        }

        /// <summary>
        /// 属性更改时的事件
        /// </summary>
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

        //事件通知
        public event EventHandler<APIEventArgs<IEnumerable<DealerModel>>> LoadDataCompleted;

        /// <summary>
        /// 请求经销商列表信息
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="id">经销商id</param>
        public void LoadDataAysnc(string url, string id, string cityId)
        {

            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }

          //  wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url+"&a="+Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<DealerModel>> apiArgs = new APIEventArgs<IEnumerable<DealerModel>>();
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
                        if (json != null)
                        {
                            JArray carJson = (JArray)json.SelectToken("result").SelectToken("dealerlist");

                            using (LocalDataContext ldc = new LocalDataContext())
                            {

                                DealerModel model = null;
                                for (int i = 0; i < carJson.Count; i++)
                                {
                                    model = new DealerModel();
                                    //车型id
                                    model.CarId = int.Parse(id);
                                    //城市id
                                    model.CityId = int.Parse(cityId);
                                    //经销商id
                                    model.id = (int)carJson[i].SelectToken("id");
                                    model.name = (string)carJson[i].SelectToken("shortname");
                                    model.DealerPrice = (string)carJson[i].SelectToken("saleprice");
                                    model.Price = (string)carJson[i].SelectToken("price");
                                    model.Scope = (string)carJson[i].SelectToken("scopename");
                                    //model.ScopeFactory = (string)carJson[i].SelectToken("scopeFactory");
                                    model.Address = (string)carJson[i].SelectToken("address");
                                    model.Tel = (string)carJson[i].SelectToken("phone");
                                    //model.LinkPeople = (string)carJson[i].SelectToken("linkPeople");
                                    model.StyledTel = (string)carJson[i].SelectToken("phonestyled");
                                    model.MapLon = (string)carJson[i].SelectToken("lat");
                                    model.MapXon = (string)carJson[i].SelectToken("lon");
                                    DealerDataSource.Add(model);
                                    //将经销商列表存入本地数据库中
                                    ldc.dealerModels.InsertOnSubmit(model);

                                }
                                //提交到数据库
                                ldc.SubmitChanges();

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }

                //注意
                apiArgs.Result = DealerDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });
        }


    }
}

using System;
using System.Net;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Model;
using ViewModels.Handler;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ViewModels
{
    public class SpecAlibiViewModel : BindableBase
    {
        public SpecAlibiViewModel()
        {
        }
        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private SpecAlibiModel _DataSource;
        public SpecAlibiModel DataSource
        {
            get
            {
                return _DataSource;
            }
            set
            {
                if (value != _DataSource)
                {
                    _DataSource = value;
                    OnPropertyChanged("DataSource");
                }
            }
        }

        //事件通知
        public event EventHandler<APIEventArgs<SpecAlibiModel>> LoadDataCompleted;

        public void LoadDataAysnc(string url)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }

            // wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url + "&a=" + Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<SpecAlibiModel> apiArgs = new APIEventArgs<SpecAlibiModel>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    JObject json = JObject.Parse(ee.Result);
                    JToken result = json.SelectToken("result");
                    if (result != null)
                    {
                        SpecAlibiModel model = new SpecAlibiModel();
                        //model.Grade = (string)result.SelectToken("grade");
                        //model.PeopleNum = (int)result.SelectToken("peoplenum");
                        //model.HasStopSellAlibi = (int)result.SelectToken("ishasstopsellalibi");

                        //var ja = (JArray)result.SelectToken("list");
                        //if (ja != null && ja.Count > 0)
                        //{
                        //    var groupList = new List<CarSeriesAlibiSpecGroupModel>();
                        //    foreach (var item in ja)
                        //    {
                        //        CarSeriesAlibiSpecGroupModel specGroup = new CarSeriesAlibiSpecGroupModel();
                        //        specGroup.key = (string)item.SelectToken("name");

                        //        var ja2 = (JArray)item.SelectToken("specs");
                        //        if (ja2 != null && ja2.Count > 0)
                        //        {
                        //            var specList = new List<CarSeriesAlibiSpecModel>();
                        //            foreach (var item2 in ja2)
                        //            {
                        //                CarSeriesAlibiSpecModel spec = new CarSeriesAlibiSpecModel();
                        //                spec.ID = (int)item2.SelectToken("specid");
                        //                spec.Name = (string)item2.SelectToken("specname");
                        //                spec.Grade = (string)item2.SelectToken("grade");
                        //                spec.PeopleNum = (int)item2.SelectToken("peoplenum");
                        //                spec.FlowModeName = (string)item2.SelectToken("flowmodename");
                        //                spec.Displacement = (double)item2.SelectToken("displacement");
                        //                spec.EnginePower = (string)item2.SelectToken("enginepower");
                        //                spec.Year = (string)item2.SelectToken("year");
                        //                specGroup.Add(spec);
                        //            }
                        //        }
                        //        groupList.Add(specGroup);
                        //    }
                        //    model.SpecGroupList = groupList;
                        //}
                        DataSource = model;
                    }
                }

                //返回结果集
                apiArgs.Result = DataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });
        }


    }
}

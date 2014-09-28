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
    public class AlibiDetailViewModel : BindableBase
    {
        public AlibiDetailViewModel()
        {
        }

        private AlibiDetailModel _dataSource;
        public AlibiDetailModel DataSource
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

        public event EventHandler<APIEventArgs<AlibiDetailModel>> LoadDataCompleted;

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

            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<AlibiDetailModel> apiArgs = new APIEventArgs<AlibiDetailModel>();
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

                        AlibiDetailModel model = null;
                        if (result != null)
                        {
                            model = new AlibiDetailModel();
                            model.id = (int)result.SelectToken("id");
                            model.memberid = (int)result.SelectToken("memberid");
                            model.membername = (string)result.SelectToken("membername");
                            model.membericon = (string)result.SelectToken("membericon");
                            model.isauth = (int)result.SelectToken("isauth");
                            model.specid = (int)result.SelectToken("specid");
                            model.specname = (string)result.SelectToken("specname");
                            model.reportdate = (string)result.SelectToken("reportdate");
                            model.koubeitypes = (string)result.SelectToken("koubeitypes");
                            model.commentcount = (int)result.SelectToken("commentcount");
                            model.helpfulcount = (int)result.SelectToken("helpfulcount");
                            model.viewcount = (int)result.SelectToken("viewcount");
                            model.content = (string)result.SelectToken("content");

                            //carinfo
                            var cj = result.SelectToken("carinfo");
                            if (cj != null)
                            {
                                AlibiDetailCarInfoModel cinfo = new AlibiDetailCarInfoModel();
                                cinfo.boughtprice = (double)cj.SelectToken("boughtprice");
                                cinfo.boughtdate = (string)cj.SelectToken("boughtdate");
                                cinfo.boughtaddress = (string)cj.SelectToken("boughtaddress");
                                cinfo.oilconsumption = double.Parse((string)cj.SelectToken("oilconsumption"));
                                cinfo.drivenkiloms = (string)cj.SelectToken("drivenkiloms");
                                cinfo.purposes = (string)cj.SelectToken("purposes");
                                cinfo.space = double.Parse((string)cj.SelectToken("space"));
                                cinfo.power = double.Parse((string)cj.SelectToken("power"));
                                cinfo.maneuverability = double.Parse((string)cj.SelectToken("maneuverability"));
                                cinfo.actualoilcomsumption = double.Parse((string)cj.SelectToken("actualoilcomsumption"));
                                cinfo.comfortabelness = double.Parse((string)cj.SelectToken("comfortabelness"));
                                cinfo.apperance = double.Parse((string)cj.SelectToken("apperance"));
                                cinfo.inside = double.Parse((string)cj.SelectToken("internal"));
                                cinfo.costefficient = double.Parse((string)cj.SelectToken("costefficient"));

                                model.carinfo = cinfo;
                            }

                            var ja = (JArray)result.SelectToken("piclist");
                            if (ja != null && ja.Count > 0)
                            {
                                var list = new List<AlibiDetailPicModel>();
                                foreach (var item in ja)
                                {
                                    AlibiDetailPicModel pm = new AlibiDetailPicModel();
                                    pm.bigurl = (string)item.SelectToken("bigurl");
                                    pm.smallurl = (string)item.SelectToken("smallurl");
                                    list.Add(pm);
                                }
                                model.piclist = list;
                            }

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

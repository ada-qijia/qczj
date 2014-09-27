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

                        model.Average = (string)result.SelectToken("average");
                        model.PeopleNum = (int)result.SelectToken("peoplenum");
                        model.FuelConsumption = (string)result.SelectToken("fuelconsumption");
                        model.OilPeopleNum = (int)result.SelectToken("oilpeoplenum");
                        model.LevelName = (string)result.SelectToken("levelname");
                        model.PageIndex = (int)result.SelectToken("pageindex");
                        model.PageCount = (int)result.SelectToken("pagecount");
                        model.PageSize = (int)result.SelectToken("pagesize");
                        model.Total = (int)result.SelectToken("total");

                        JArray ja = null;
                        ja = (JArray)result.SelectToken("grades");
                        if (ja != null && ja.Count > 0)
                        {
                            var gradeList = new Dictionary<string, AlibiGradeModel>();
                            foreach (var item in ja)
                            {
                                AlibiGradeModel agm = new AlibiGradeModel();
                                agm.Name = (string)item.SelectToken("name");
                                agm.Grade = double.Parse((string)item.SelectToken("grade"));
                                agm.LevelGrade = double.Parse((string)item.SelectToken("levelgrade"));
                                gradeList.Add(agm.Name, agm);
                            }
                            model.Grades = gradeList;
                        }

                        ja = null;
                        ja = (JArray)result.SelectToken("koubeis");
                        if (ja != null && ja.Count > 0)
                        {
                            var koubeiList = new List<KoubeiModel>();
                            foreach (var item in ja)
                            {
                                KoubeiModel koubei = new KoubeiModel();
                                koubei.ID = (int)item.SelectToken("id");
                                koubei.SpecName = (string)item.SelectToken("specname");

                                //medal
                                var mj = item.SelectToken("medals");
                                if (mj != null)
                                {
                                    KoubeiMedalModel medal = new KoubeiMedalModel();
                                    medal.Name = (string)mj.SelectToken("name");
                                    medal.Type = (int)mj.SelectToken("type");
                                    koubei.Medals = medal;
                                }

                                koubei.UserName = (string)item.SelectToken("username");
                                koubei.UserID = (int)item.SelectToken("userid");
                                koubei.IsAuth = (int)item.SelectToken("isauth");
                                koubei.PostTime = (string)item.SelectToken("posttime");
                                koubei.Content = (string)item.SelectToken("content");
                                koubei.UserPic = (string)item.SelectToken("userpic");
                                koubei.SeriesName = (string)item.SelectToken("seriesname");

                                koubeiList.Add(koubei);
                            }
                            model.Koubeis = koubeiList;
                        }

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

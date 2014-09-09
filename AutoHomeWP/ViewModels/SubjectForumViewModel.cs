using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;

namespace ViewModels
{
    public class SubjectForumViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public SubjectForumViewModel()
        {
            _subjectForumDataSource = new ObservableCollection<SubjectForumModel>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<SubjectForumModel> _subjectForumDataSource;
        public ObservableCollection<SubjectForumModel> SubjectForumDataSource
        {
            get
            {
                return _subjectForumDataSource;
            }
            set
            {
                if (value != _subjectForumDataSource)
                {
                    OnPropertyChanging("SubjectForumDataSource");
                    _subjectForumDataSource = value;
                    OnPropertyChanged("SubjectForumDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<SubjectForumModel>>> LoadDataCompleted;


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

            //wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<SubjectForumModel>> apiArgs = new APIEventArgs<IEnumerable<SubjectForumModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    JArray areaforumJson = (JArray)json.SelectToken("result").SelectToken("list");

                    using (LocalDataContext ldc = new LocalDataContext())
                    {

                        SubjectForumModel model = null;
                        for (int i = 0; i < areaforumJson.Count; i++)
                        {
                            model = new SubjectForumModel();
                            model.bbsName = (string)areaforumJson[i].SelectToken("bbsname");
                            model.bbsId = (int)areaforumJson[i].SelectToken("bbsid");
                            model.bbsType = (string)areaforumJson[i].SelectToken("bbstype");
                            model.CreateTime = DateTime.Now;
                            SubjectForumDataSource.Add(model);

                            ldc.subjectForums.InsertOnSubmit(model);


                        }
                        //提交到数据库
                        ldc.SubmitChanges();
                    }
                }

                //注意
                apiArgs.Result = SubjectForumDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });

        }
    }
}

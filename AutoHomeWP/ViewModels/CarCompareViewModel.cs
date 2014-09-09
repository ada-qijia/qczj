using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;

namespace ViewModels
{
    public class CarCompareViewModel
    {
        public int Id { get; set; }
        public int SpecId { get; set; }
        public string SpecName { get; set; }
        public int SeriesId { get; set; }
        public string SeriesName { get; set; }
        public bool IsChoosed { get; set; }
        /// <summary>
        /// 添加对比车型
        /// </summary>
        public int AddCompareSpec(CarCompareViewModel model)
        {
            //操作状态码：0,添加成功;1,此车型已存在;2,只能添加9个车型
            int returncode = 1;
            if (model != null)
            {
                CarCompareModelT m = new CarCompareModelT()
                {
                    IsChoosed = model.IsChoosed,
                    SpecId = model.SpecId,
                    SpecName = model.SpecName,
                    SeriesId = model.SeriesId,
                    SeriesName = model.SeriesName
                };

                using (LocalDataContext ldc = new LocalDataContext())
                {
                    var group = from s in ldc.carCompareT where s.SpecId > 0 select s;
                    if (group.Count() >= 9) //对比库已满
                        returncode = 2;
                    else if (group.Count() <= 0) //对比库无数据
                    {
                        ldc.carCompareT.InsertOnSubmit(m);
                        ldc.SubmitChanges();
                        returncode = 0;
                    }
                    else //对比库还可以添加数据
                    {
                        if (group.Count(c => c.SpecId == m.SpecId) > 0)//已经有此数据，不能插入
                            returncode = 1;
                        else
                        {
                            ldc.carCompareT.InsertOnSubmit(m);
                            ldc.SubmitChanges();
                            returncode = 0;
                        }
                    }
                }
            }
            else
            { }
            return returncode;
        }
        /// <summary>
        /// 删除对比车型
        /// </summary>
        public void DeleteCompareSpec(List<int> SpecIds)
        {
            using (LocalDataContext ldc = new LocalDataContext())
            {
                foreach (int item in SpecIds)
                {
                    var oc = from s in ldc.carCompareT where s.SpecId == item select s;
                    ldc.carCompareT.DeleteAllOnSubmit(oc);
                }
                ldc.SubmitChanges();
            }
        }
        /// <summary>
        /// 删除指定车型ID对比车型
        /// <param name="SpecId">车型id</param>
        /// </summary>
        public void DeleteCompareSpecById(int SpecId)
        {
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var oc = from s in ldc.carCompareT where s.SpecId == SpecId select s;
                ldc.carCompareT.DeleteAllOnSubmit(oc);
                ldc.SubmitChanges();
            }
        }
        /// <summary>
        /// 删除选中/未选中的对比车型
        /// <param name="IsSelected">true:删除选中的;fale删除未选中的</param>
        /// </summary>
        public void DeleteSelectEdCompareSpec(bool IsSelected)
        {
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var oc = from s in ldc.carCompareT where s.IsChoosed == IsSelected select s;
                ldc.carCompareT.DeleteAllOnSubmit(oc);
                ldc.SubmitChanges();
            }
        }
        /// <summary>
        /// 获取对比车型
        /// <param name="State">0:所有车型;1,选中的车型;2未选中的车型</param>
        /// </summary>
        public ObservableCollection<CarCompareViewModel> GetCompareSpec(int State = 0)
        {
            ObservableCollection<CarCompareViewModel> oc = new ObservableCollection<CarCompareViewModel>();
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var list = from s in ldc.carCompareT where s.SpecId > 0 select s;
                if (State == 1)
                    list = from s in ldc.carCompareT where s.SpecId > 0 && s.IsChoosed select s;
                else if (State == 2)
                    list = from s in ldc.carCompareT where s.SpecId > 0 && !s.IsChoosed select s;
                else
                { }
                if (list != null && list.Count() > 0)
                {
                    CarCompareViewModel model;
                    foreach (var n in list)
                    {
                        model = new CarCompareViewModel()
                        {
                            SpecId = n.SpecId,
                            SpecName = n.SpecName,
                            SeriesId = n.SeriesId,
                            SeriesName = n.SeriesName,
                            IsChoosed = n.IsChoosed
                        };
                        oc.Add(model);
                    }
                }
            }
            return oc;
        }
        /// <summary>
        /// 更新对比车型
        /// </summary>
        public void UpdateCompareSpec(int SpecId, bool IsChoosed)
        {
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var item = ldc.carCompareT.FirstOrDefault(f => f.SpecId == SpecId);
                if (item != null && item.SpecId > 0)
                {
                    item.IsChoosed = IsChoosed;
                }
                ldc.SubmitChanges();
            }

        }
    }

    public class CarCompareInfoViewModel : List<ItemInfo>
    {
        public CarCompareInfoViewModel()
        { }
        public CarCompareInfoViewModel(string key)
        {
            GroupName = key;
        }
        public string GroupName
        {
            get;
            set;
        }
        //事件通知
        public event EventHandler<APIEventArgs<IEnumerable<CarCompareInfoViewModel>>> LoadDataCompleted;
        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        public void LoadDataAysnc(string url, List<int> specList)
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
                ObservableCollection<CarCompareInfoViewModel> infoM = new ObservableCollection<CarCompareInfoViewModel>();
                APIEventArgs<IEnumerable<CarCompareInfoViewModel>> apiArgs = new APIEventArgs<IEnumerable<CarCompareInfoViewModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);

                    JArray carJson = (JArray)json.SelectToken("result").SelectToken("paramitems");

                    CarCompareInfoViewModel carCompareInfoVM;
                    for (int i = 0; i < carJson.Count; i++)
                    {
                        carCompareInfoVM = new CarCompareInfoViewModel((string)carJson[i].SelectToken("itemtype"));
                        JArray carItemJson = (JArray)carJson[i].SelectToken("items");
                        ItemInfo itemInfo;
                        for (int j = 0; j < carItemJson.Count; j++)
                        {
                            itemInfo = new ItemInfo();
                            itemInfo.ItemName = (string)carItemJson[j].SelectToken("name");
                            JArray infoJson = (JArray)carItemJson[j].SelectToken("modelexcessids");
                            itemInfo.ItemValue1 = "";
                            itemInfo.ItemValue2 = "";
                            for (int k = 0; k < infoJson.Count; k++)
                            {
                                if ((int)infoJson[k].SelectToken("id") == specList[0])
                                {
                                    itemInfo.ItemValue1 = (string)infoJson[k].SelectToken("value");
                                }
                                if (specList.Count >= 2 && (int)infoJson[k].SelectToken("id") == specList[1])
                                {
                                    itemInfo.ItemValue2 = (string)infoJson[k].SelectToken("value");
                                }
                            }
                            carCompareInfoVM.Add(itemInfo);
                        }
                        infoM.Add(carCompareInfoVM);
                    }
                }

                //返回结果集
                apiArgs.Result = infoM;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });
        }
    }
    public class ItemInfo : BaseViewModel
    {
        private string _itemname;
        public string ItemName
        {
            get
            {
                return _itemname;
            }
            set
            {
                if (value != _itemname)
                {
                    OnPropertyChanging("ItemName");
                    _itemname = value;
                    OnPropertyChanged("ItemName");
                }
            }
        }
        private string _itemvalue1;
        public string ItemValue1
        {
            get
            {
                return _itemvalue1;
            }
            set
            {
                if (value != _itemvalue1)
                {
                    OnPropertyChanging("ItemValue1");
                    _itemvalue1 = value;
                    OnPropertyChanged("ItemValue1");
                }
            }
        }
        private string _itemvalue2;
        public string ItemValue2
        {
            get
            {
                return _itemvalue2;
            }
            set
            {
                if (value != _itemvalue2)
                {
                    OnPropertyChanging("ItemValue2");
                    _itemvalue2 = value;
                    OnPropertyChanged("ItemValue2");
                }
            }
        }
    }
}

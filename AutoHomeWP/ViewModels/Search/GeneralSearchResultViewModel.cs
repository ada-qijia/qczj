using CommonLayer;
using Model.Search;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace ViewModels.Search
{
    public class GeneralSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public GeneralSearchResultViewModel()
        {
            //只有精选和自然结果会有多页
            this.JingxuanList = new ObservableCollection<JingxuanModel>();
            this.NaturalResultList = new ObservableCollection<NaturalModel>();
        }

        #region properties

        private GeneralSearchResultMatchType _matchType;
        public GeneralSearchResultMatchType MatchType
        {
            get { return _matchType; }
            set { SetProperty<GeneralSearchResultMatchType>(ref _matchType, value); }
        }

        private HotSeriesModel _series;
        /// <summary>
        /// 车系内容块, 关键词为车系/车型时返回
        /// </summary>
        public HotSeriesModel Series
        {
            get { return _series; }
            set { SetProperty<HotSeriesModel>(ref _series, value); }
        }

        private List<SpecModel> _specList;
        /// <summary>
        /// 车型内容块, 关键词为车系/车型时返回,最多3款
        /// </summary>
        public List<SpecModel> SpecList
        {
            get { return _specList; }
            set { SetProperty<List<SpecModel>>(ref _specList, value); }
        }

        private List<SeriesModel> _seriesList;
        /// <summary>
        /// 品牌/厂商内容块，关键词为厂商/品牌时返回，最多5条
        /// </summary>
        public List<SeriesModel> SeriesList
        {
            get { return _seriesList; }
            set { SetProperty<List<SeriesModel>>(ref _seriesList, value); }
        }

        private int _imgCount;
        public int ImgCount
        {
            get { return _imgCount; }
            set { SetProperty<int>(ref _imgCount, value); }
        }

        private List<ImgModel> _imgList;
        /// <summary>
        /// 图片块，关键词为车系/车型时返回，头4张
        /// </summary>
        public List<ImgModel> ImgList
        {
            get { return _imgList; }
            set { SetProperty<List<ImgModel>>(ref _imgList, value); }
        }

        private List<RelatedSeriesModel> _relatedSeriesList;
        /// <summary>
        /// 相关推荐内容块，关键词为车系时返回
        /// </summary>
        public List<RelatedSeriesModel> RelatedSeriesList
        {
            get { return _relatedSeriesList; }
            set { SetProperty<List<RelatedSeriesModel>>(ref _relatedSeriesList, value); }
        }

        private List<DealerSearchModel> _dealerList;
        /// <summary>
        /// 经销商内容块，关键词为厂商时返回
        /// </summary>
        public List<DealerSearchModel> DealerList
        {
            get { return _dealerList; }
            set { SetProperty<List<DealerSearchModel>>(ref _dealerList, value); }
        }

        private List<FindSeriesModel> _findSeriesList;
        /// <summary>
        /// 找车内容块，关键词为找车时返回
        /// </summary>
        public List<FindSeriesModel> FindSeriesList
        {
            get { return _findSeriesList; }
            set { SetProperty<List<FindSeriesModel>>(ref _findSeriesList, value); }
        }

        /// <summary>
        /// 论坛精选，关键词为论坛时返回
        /// </summary>
        public ObservableCollection<JingxuanModel> JingxuanList { get; private set; }

        /// <summary>
        ///  其他自然结果内容块，关键词为车系/车型/厂商/品牌/找车时返回
        /// </summary>
        public ObservableCollection<NaturalModel> NaturalResultList { get; private set; }

        #endregion

        #region interface implementation

        public event EventHandler LoadDataCompleted;

        private bool isLoading = false;
        public void LoadDataAysnc(string url)
        {
            if (!isLoading)
            {
                isLoading = true;

                this.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
                {
                    if (ee.Error == null && ee.Result != null)
                    {
                        try
                        {
                            //返回的json数据
                            JObject json = JObject.Parse(ee.Result);
                            JToken resultToken = json.SelectToken("result");                           

                            #region 用返回结果填充每个版块

                            int type = resultToken.SelectToken("matchtype").Value<int>();
                            if (Enum.IsDefined(typeof(GeneralSearchResultMatchType), type))
                            {
                                MatchType = (GeneralSearchResultMatchType)type;
                            }

                            this.RowCount = resultToken.SelectToken("rowcount").Value<int>();
                            this.PageIndex = resultToken.SelectToken("pageindex").Value<int>();

                            JToken blockToken;
                            //车系
                            blockToken = resultToken.SelectToken("seriesmodel");
                            if (blockToken.HasValues)
                            {
                                var seriesModel = JsonHelper.DeserializeOrDefault<HotSeriesModel>(blockToken.ToString());
                                if (seriesModel != null && seriesModel.IsShowModel)
                                {
                                    this.Series = seriesModel; 
                                }
                            }

                            //车型
                            blockToken = resultToken.SelectToken("speclistmodel");
                            if (blockToken.HasValues)
                            {
                                this.SpecList = JsonHelper.DeserializeOrDefault<List<SpecModel>>(blockToken.ToString());
                            }

                            //品牌
                            blockToken = resultToken.SelectToken("serieslistmodel");
                            if (blockToken.HasValues)
                            {
                                this.SeriesList = JsonHelper.DeserializeOrDefault<List<SeriesModel>>(blockToken.ToString());
                            }

                            //图片
                            blockToken = resultToken.SelectToken("imgmodel");
                            if (blockToken.HasValues)
                            {
                                this.ImgCount = blockToken.SelectToken("imgcount").Value<int>();
                                this.ImgList = JsonHelper.DeserializeOrDefault<List<ImgModel>>(blockToken.SelectToken("imglist").ToString());
                            }

                            // RelatedSeriesList
                            blockToken = resultToken.SelectToken("relatedseriesmodel");
                            if (blockToken.HasValues)
                            {
                                this.RelatedSeriesList = JsonHelper.DeserializeOrDefault<List<RelatedSeriesModel>>(blockToken.ToString());
                            }

                            //DealerList
                            blockToken = resultToken.SelectToken("dealermodel");
                            if (blockToken.HasValues)
                            {
                                this.DealerList = JsonHelper.DeserializeOrDefault<List<DealerSearchModel>>(blockToken.ToString());
                            }

                            // FindCarResult
                            blockToken = resultToken.SelectToken("findcarmodel.findserieslist");
                            if (blockToken != null && blockToken.HasValues)
                            {
                                this.FindSeriesList = JsonHelper.DeserializeOrDefault<List<FindSeriesModel>>(blockToken.ToString());
                            }

                            //JingxuanList
                            blockToken = resultToken.SelectToken("jingxuanmodel");
                            if (blockToken.HasValues)
                            {
                                var modelList = JsonHelper.DeserializeOrDefault<List<JingxuanModel>>(blockToken.ToString());
                                if (modelList != null)
                                {
                                    foreach (var model in modelList)
                                    {
                                        this.JingxuanList.Add(model);
                                    }
                                }
                            }

                            //NaturalResultList
                            blockToken = resultToken.SelectToken("naturemodel");
                            if (blockToken.HasValues)
                            {
                                var naturalList = JsonHelper.DeserializeOrDefault<List<NaturalModel>>(blockToken.ToString());
                                if (naturalList != null)
                                {
                                    foreach (var naturalModel in naturalList)
                                    {
                                        this.NaturalResultList.Add(naturalModel);
                                    }
                                }
                            }

                            #endregion
                        }
                        catch
                        {
                        }
                    }

                    isLoading = false;

                    //触发完成事件
                    if (LoadDataCompleted != null)
                    {
                        LoadDataCompleted(this, null);
                    }
                });

                //开始下载
                this.DownloadStringAsync(url);
            }
        }

        public void ClearData()
        {
            this.MatchType = GeneralSearchResultMatchType.Others;
            this.Series = null;
            this.SpecList = null;
            this.SeriesList = null;
            this.ImgCount = 0;
            this.ImgList = null;
            this.RelatedSeriesList = null;
            this.DealerList = null;
            this.FindSeriesList = null;
            this.JingxuanList.Clear();
            this.NaturalResultList.Clear();
        }

        #endregion
    }
}

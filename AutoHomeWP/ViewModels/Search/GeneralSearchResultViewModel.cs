using CommonLayer;
using Model.Search;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using ViewModels.Handler;

namespace ViewModels.Search
{
    public class GeneralSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public GeneralSearchResultViewModel()
        {
            this.NaturalResultList = new ObservableCollection<NaturalModel>();
        }

        #region properties

        public GeneralSearchResultMatchType MatchType { get; set; }

        /// <summary>
        /// 车系内容块, 关键词为车系/车型时返回
        /// </summary>
        public HotSeriesModel Series { get; set; }

        /// <summary>
        /// 车型内容块, 关键词为车系/车型时返回,最多3款
        /// </summary>
        public List<SpecModel> SpecList { get; set; }

        /// <summary>
        /// 品牌/厂商内容块，关键词为厂商/品牌时返回，最多5条
        /// </summary>
        public List<SeriesModel> SeriesList { get; set; }

        /// <summary>
        /// 图片块，关键词为车系/车型时返回，头4张
        /// </summary>
        public ImgsModel Imgs { get; set; }

        /// <summary>
        /// 相关推荐内容块，关键词为车系时返回
        /// </summary>
        public List<RelatedSeriesModel> RelatedSeriesList { get; set; }

        /// <summary>
        /// 经销商内容块，关键词为厂商时返回
        /// </summary>
        public List<DealerSearchModel> DealerList { get; set; }

        /// <summary>
        /// 找车内容块，关键词为找车时返回
        /// </summary>
        public FindCarModel FindCarResult { get; set; }

        /// <summary>
        /// 论坛精选，关键词为论坛时返回
        /// </summary>
        public List<JingxuanModel> JingxuanList { get; set; }

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

                            int type = resultToken.SelectToken("matchtype").Value<int>();
                            MatchType = (GeneralSearchResultMatchType)type;

                            #region 用返回结果填充每个版块

                            JToken blockToken;
                            //车系
                            blockToken = resultToken.SelectToken("seriesmodel");
                            if (blockToken.HasValues)
                            {
                                this.Series = JsonHelper.DeserializeOrDefault<HotSeriesModel>(blockToken.ToString());
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
                                this.Imgs = JsonHelper.DeserializeOrDefault<ImgsModel>(blockToken.ToString());
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
                            blockToken = resultToken.SelectToken("findcarmodel");
                            if (blockToken.HasValues)
                            {
                                this.FindCarResult = JsonHelper.DeserializeOrDefault<FindCarModel>(blockToken.ToString());
                            }

                            //JingxuanList
                            blockToken = resultToken.SelectToken("jingxuanmodel");
                            if (blockToken.HasValues)
                            {
                                this.JingxuanList = JsonHelper.DeserializeOrDefault<List<JingxuanModel>>(blockToken.ToString());
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
            throw new NotImplementedException();
        }

        #endregion
    }
}

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
using System.Data.Linq;
using Model;

namespace ViewModels.Handler
{
    public class LocalDataContext : DataContext
    {
        public LocalDataContext()
            : base("Data Source='isostore:/MyDB.sdf'")
        {

        }

        // 最新
        public Table<NewsModel> newestModels
        {
            get
            {
                return GetTable<NewsModel>();
            }
        }


        // 经销商
        public Table<DealerModel> dealerModels
        {
            get
            {
                return GetTable<DealerModel>();
            }
        }


        // 省市
        public Table<ProvinceModel> provinces
        {
            get
            {
                return GetTable<ProvinceModel>();
            }
        }

        // 汽车品牌集合
        public Table<CarBrandModel> carBrandModels
        {
            get
            {
                return GetTable<CarBrandModel>();
            }
        }


        //车系表
        public Table<CarSeriesModel> carSeries
        {
            get
            {
                return GetTable<CarSeriesModel>();
            }
        }

        //车型表
        public Table<CarSeriesQuoteModel> carQuotes
        {

            get
            {
                return GetTable<CarSeriesQuoteModel>();
            }
        }

        //车系论坛表
        public Table<carSeriesAllForumModel> carSeriesForums
        {
            get
            {
                return GetTable<carSeriesAllForumModel>();
            }
        }

        //地区论坛
        public Table<AreaForumModel> areaForums
        {
            get
            {
                return GetTable<AreaForumModel>();
            }
        }

        //主题论坛
        public Table<SubjectForumModel> subjectForums
        {
            get
            {
                return GetTable<SubjectForumModel>();
            }
        }

        //我的论坛
        public Table<MyForumModel> myForum
        {
            get
            {
                return GetTable<MyForumModel>();
            }
        }
        /// <summary>
        /// 存储需要在客户端缓存的数据
        /// </summary>
        public Table<CacheVarModel> publicValue
        {
            get
            {
                return GetTable<CacheVarModel>();
            }
        }
        /// <summary>
        /// 获取对比表
        /// </summary>
        public Table<CarCompareModelT> carCompareT
        {
            get
            {
                return GetTable<CarCompareModelT>();
            }
        }
    }
}

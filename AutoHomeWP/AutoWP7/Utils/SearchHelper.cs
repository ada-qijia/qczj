using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWP7.Utils
{
    public enum SearchType
    {
        //综合
        General,
        //视频
        Video,
        //论坛
        Forum,
        //车系
        Cars,
        //文章
        Article,
    }

    public class SearchHelper
    {
        public static string GetSearchPageUrlWithParams(SearchType type)
        {
            return string.Format("/View/Search/SearchPage.xaml?type={0}", (int)type);
        }
    }
}

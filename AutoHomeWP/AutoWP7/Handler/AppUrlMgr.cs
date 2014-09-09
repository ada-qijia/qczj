using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWP7.Handler
{
    public class AppUrlMgr
    {
        /// <summary>
        /// 帖子最终页Url
        /// 需要值：topicid,owner,pageindex,pagesize,ishowpage,nightmodel,fontsize,smallpicmodel,floor,clientwidth
        /// </summary>
        public static string TopicWebViewUrl(long topicid, int owner, int page, int pagesize, int isshowpage, int isnightmodel, int fontsize, int issmallpic, int floor, int clientWith,int issend)
        {
            string url = App.topicPageDomain + App.versionStr + "/forum/club/topiccontent-" + App.AppInfo + "-t{0}-o{1}-p{2}-s{3}-c{4}-nt{5}-fs{6}-sp{7}-al{8}-cw{9}-r{10}.html";
            return string.Format(url, topicid, owner, page, pagesize, isshowpage, isnightmodel, fontsize, issmallpic, floor, "", issend);
        }
        /// <summary>
        /// 文章最终页Url
        /// <param name="fontsizemodel">0:普通，1大，2小</param>
        /// </summary>
        public static string NewsWebViewUrl(int newsid, int islazyload, int issmallmode, int isnight, int isshowad, int page, int isshowbody, int fontsizemodel, int clientWith)
        {
            string url = App.newsPageDomain + App.versionStr + "/content/news/newscontent-" + App.AppInfo + "-n{0}-lz{1}-sp{2}-nt{3}-sa{4}-p{5}-c{6}-fs{7}-cw.html";
            return string.Format(url, newsid, islazyload, issmallmode, isnight, isshowad, page, isshowbody, fontsizemodel, clientWith);
        }
        /// <summary>
        /// 说课最终页Url
        /// <param name="fontsizemodel">0:普通，1大，2小</param>
        /// </summary>
        public static string ShuoWebViewUrl(int newsid, int isnight, int islazyload, int page, int fontsize, int clienWith)
        {
            string url = App.newsPageDomain + App.versionStr + "/content/news/shuokecontent-" + App.AppInfo + "-n{0}-nt{1}-lz{2}-p{3}-fs{4}-cw{5}.html";
            return string.Format(url, newsid, isnight, islazyload, page, fontsize, "");
        }
        /// <summary>
        /// 帖子列表接口Url
        /// bbsid,bbstype,isrefine,seriesid,order,page,pagesize
        /// </summary>
        public static string TopicsUrl
        {
            get
            {
                return App.appUrl + App.versionStr + "/club/topics-" + App.AppInfo + "-b{0}-bt{1}-r{2}-ss{3}-o{4}-p{5}-s{6}.html";
            }
        }
        /// <summary>
        /// 车系车型图片Url
        /// seriesid,specid,category,color,page,pagesize
        /// </summary>
        public static string CarPicsUrl
        {
            get
            {
                return App.appUrl + App.versionStr + "/cars/pics-" + App.AppInfo + "-ss{0}-sp{1}-cg{2}-cl{3}-p{4}-s{5}.html";
            }
        }
        /// <summary>
        /// 资讯文章列表Url
        /// cityid,newstype,page,pagesize,lasttime
        /// </summary>
        public static string NewsListUrl
        {
            get
            {
                return App.appUrl + App.versionStr + "/news/newslist-" + App.AppInfo + "-c{0}-nt{1}-p{2}-s{3}-l{4}.html";
            }
        }
    }
}

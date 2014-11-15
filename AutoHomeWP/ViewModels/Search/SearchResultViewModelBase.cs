using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Search
{
    public abstract class SearchResultViewModelBase
    {
        protected event DownloadStringCompletedEventHandler DownloadStringCompleted;
        
        public void DownloadStringAsync(string url)
        {
            WebClient wc = new WebClient();

            wc.Encoding = System.Text.Encoding.UTF8;
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
  
            wc.DownloadStringCompleted+= new DownloadStringCompletedEventHandler((obj, e) =>
            { 
                if(this.DownloadStringCompleted!=null)
                {
                    this.DownloadStringCompleted(this, e);
                }
            });

            Uri urlSource = new Uri(url + "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
        }
    }
}

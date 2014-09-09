using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWP7.Handler
{
    public class MutiResImageChooser
    {
        public Uri BestResolutionImage
        {
            get
            {
                switch (ResolutionHelper.CurrentResolution)
                {
                    case Resolutions.HD720p:
                        return  new Uri("",UriKind.Relative);
                    case Resolutions.WVGA:
                        return new Uri("", UriKind.Relative);
                    case Resolutions.WXGA:
                        return new Uri("", UriKind.Relative);
                    default :
                        throw new InvalidOperationException("unknown resolution type");
                }
            }
        }
    }


}

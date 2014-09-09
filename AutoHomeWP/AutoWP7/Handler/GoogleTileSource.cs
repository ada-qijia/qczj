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
using Microsoft.Phone.Controls.Maps;

namespace AutoWP7.Handler
{
    public class GoogleTileSource:TileSource
    {
        public GoogleTileSource()
            : base("http://mt{0}.google.com/vt/lyrs=m@107&hl=zh&x={1}&y={2}&z={3}&s=Ga")
        {

        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            int num = ((x & 1) << 1) | (y & 1);

            return new Uri(string.Format(this.UriFormat, num, x, y, zoomLevel));
        }
           
    }
}

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
using Newtonsoft.Json.Linq;
using Model;

namespace ViewModels.Handler
{
    public class APIEventArgs<T>:EventArgs
    {
        public Exception Error { get; set; }

        public T  Result { get; set; }

    }
}

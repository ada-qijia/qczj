using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Search
{
    public interface ISearchViewModel
    {
        event EventHandler LoadDataCompleted;

        void LoadDataAysnc(string url);

        void ClearData();
    }
}

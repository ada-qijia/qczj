using Model.Me;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Me
{
    public class DraftViewModel : Model.BindableBase
    {
        public DraftViewModel()
        {
            this.DraftList = new ObservableCollection<DraftModel>();
        }

        #region properties

        private int _unreadCount;
        public int UnReadCount
        {
            get { return _unreadCount; }
            set
            { SetProperty<int>(ref _unreadCount, value); }
        }

        public ObservableCollection<DraftModel> DraftList { get; private set; }

        #endregion

        #region public methods

        public void SaveDraft(DraftModel model)
        {
            this.DraftList.Insert(0, model);
            //save file
        }

        public void RemoveDraft(List<DraftModel> models)
        {
            foreach(var model in models)
            {
                this.DraftList.Remove(model);
            }

            //save file
        }

        #endregion
    }
}

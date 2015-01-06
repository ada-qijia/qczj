﻿using CommonLayer;
using Model.Me;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ViewModels.Me
{
    public class DraftViewModel : Model.BindableBase
    {
        public static string FilePath;

        private DraftViewModel()
        {
            this.DraftList = new ObservableCollection<DraftModel>();
        }

        #region single instance

        private static DraftViewModel _singleInstance;
        public static DraftViewModel SingleInstance
        {
            get
            {
                if (_singleInstance == null)
                {
                    _singleInstance = new DraftViewModel();
                    _singleInstance.LoadDraft();
                }
                return _singleInstance;
            }
        }

        #endregion

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

        //read file
        private void LoadDraft()
        {
            string content = IsolatedStorageFileHelper.ReadIsoFile(FilePath);
            List<DraftModel> result = JsonHelper.DeserializeOrDefault<List<DraftModel>>(content);
            if (result != null)
            {
                this.DraftList.Concat(result);
            }
        }

        //add item and save file
        public bool AddDraft(DraftModel model)
        {
            this.DraftList.Insert(0, model);
            return SaveDraft();
        }

        //remove items and save file
        public bool RemoveDraft(List<DraftModel> models)
        {
            foreach (var model in models)
            {
                this.DraftList.Remove(model);
            }
            return SaveDraft();
        }

        private bool SaveDraft()
        {
            string content = JsonHelper.Serialize(this.DraftList);
            bool result = IsolatedStorageFileHelper.WriteIsoFile(FilePath, content, System.IO.FileMode.Create);
            return result;
        }

        #endregion
    }
}

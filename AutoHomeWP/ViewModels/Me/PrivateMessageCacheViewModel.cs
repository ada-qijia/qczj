using CommonLayer;
using Model.Me;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels.Me
{
    public class PrivateMessageCacheViewModel : Model.BindableBase
    {
        private const int MaxFriends = 20;
        private const int MaxMessages = 50;
        public static string FilePath;

        private PrivateMessageCacheViewModel()
        { }

        #region singleInstance

        private static PrivateMessageCacheViewModel _singleInstance;
        public static PrivateMessageCacheViewModel SingleInstance
        {
            get
            {
                if (_singleInstance == null)
                {
                    _singleInstance = new PrivateMessageCacheViewModel();
                    _singleInstance.LoadLocally();
                }
                return _singleInstance;
            }
        }

        #endregion

        #region properties

        private PrivateMessageCacheModel _cacheModel;
        public PrivateMessageCacheModel CacheModel
        {
            get { return _cacheModel; }
            set { SetProperty<PrivateMessageCacheModel>(ref _cacheModel, value); }
        }

        #endregion

        #region public methods

        //read file
        private void LoadLocally()
        {
            string content = IsolatedStorageFileHelper.ReadIsoFile(FilePath);
            this.CacheModel = JsonHelper.DeserializeOrDefault<PrivateMessageCacheModel>(content);
            //initialize
            if (this.CacheModel == null)
            {
                this.CacheModel = new PrivateMessageCacheModel();
            }

            if (this.CacheModel.Friends == null)
            {
                this.CacheModel.Friends = new List<PrivateMessageFriendModel>();
            }

            if (this.CacheModel.Messages == null)
            {
                this.CacheModel.Messages = new Dictionary<int, List<PrivateMessageModel>>();
            }
        }

        //更新朋友列表
        public bool UpdateFriends(List<PrivateMessageFriendModel> models)
        {
            if (models == null || models.Count == 0)
                return false;

            //更新朋友列表
            this.CacheModel.Friends.Clear();
            int lenghth = Math.Min(MaxFriends, models.Count);
            for (int i = 0; i < lenghth; i++)
            {
                this.CacheModel.Friends.Add(models[i]);
            }

            //更新私信列表
            List<int> removableKeys = new List<int>();
            foreach (var key in this.CacheModel.Messages.Keys)
            {
                var found = this.CacheModel.Friends.FirstOrDefault(item => item.ID == key);
                if (found == null)
                {
                    removableKeys.Add(key);
                }
            }

            foreach (var key in removableKeys)
            {
                this.CacheModel.Messages.Remove(key);
            }

            return this.SaveLocally();
        }

        //更新私信列表
        public bool UpdateMessages(int friendID, List<PrivateMessageModel> models)
        {
            if (models != null && models.Count > 0)
            {
                this.CacheModel.Messages.Remove(friendID);
                var found = this.CacheModel.Friends.FirstOrDefault(item => item.ID == friendID);
                if (found != null)
                {
                    this.CacheModel.Messages[friendID] = models.Where((model, index) => index < MaxMessages).ToList();
                    return this.SaveLocally();
                }
                else
                    return false;
            }
            return false;
        }

        //删除朋友
        public bool RemoveFriends(List<PrivateMessageFriendModel> models)
        {
            foreach (var model in models)
            {
                var found = this.CacheModel.Friends.FirstOrDefault(item => item.ID == model.ID);
                if (found != null)
                {
                    this.CacheModel.Friends.Remove(found);
                }
                this.CacheModel.Messages.Remove(model.ID);
            }
            return SaveLocally();
        }

        private bool SaveLocally()
        {
            string content = JsonHelper.Serialize(this.CacheModel);
            bool result = IsolatedStorageFileHelper.WriteIsoFile(FilePath, content, System.IO.FileMode.Create);
            return result;
        }

        #endregion
    }
}

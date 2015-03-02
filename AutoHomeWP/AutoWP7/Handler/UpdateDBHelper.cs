using Microsoft.Phone.Data.Linq;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Handler;

namespace AutoWP7.Handler
{
    public  class UpdateDBHelper
    {
        private void AddColumn<T>(int oldDBVersion, int newDBVersion, string columnName)
        {
          
            using (LocalDataContext db = new LocalDataContext())
            {
               
                DatabaseSchemaUpdater dbUpdate = db.CreateDatabaseSchemaUpdater();
                //Get database version
                int dbVersion = dbUpdate.DatabaseSchemaVersion;
                if (dbVersion == oldDBVersion)
                {
                    dbUpdate.AddColumn<T>(columnName);
                    
                    dbUpdate.DatabaseSchemaVersion = newDBVersion;
                    try
                    {
                        dbUpdate.Execute();
                    }
                    catch (Exception ex) 
                    {
                       //捕获异常，不处理
                       //原因：修改数据，在卸载安装App时，时有版本号归1情况出现（实际数据库表结构已修改），如果再次修改，则报错->闪退，暂未查明原因。
                       //效果：保证了接口数据正常加载，不闪退。
                    }
                }
                dbVersion = dbUpdate.DatabaseSchemaVersion;
            }
        }
        private void AddTable<T>(int oldDBVersion, int newDBVersion)
        {

            using (LocalDataContext db = new LocalDataContext())
            {

                DatabaseSchemaUpdater dbUpdate = db.CreateDatabaseSchemaUpdater();
                //Get database version
                int dbVersion = dbUpdate.DatabaseSchemaVersion;
                if (dbVersion == oldDBVersion)
                {
                    dbUpdate.AddTable<T>();

                    dbUpdate.DatabaseSchemaVersion = newDBVersion;
                    try
                    {
                        dbUpdate.Execute();
                    }
                    catch
                    {
                        //捕获异常，不处理
                        //原因：修改数据，在卸载安装App时，时有版本号归1情况出现（实际数据库表结构已修改），如果再次修改，则报错->闪退，暂未查明原因。
                        //效果：保证了接口数据正常加载，不闪退。
                    }
                }
                dbVersion = dbUpdate.DatabaseSchemaVersion;
            }
        }
        /// <summary>
        /// 1.4版本对数据库的更改
        /// </summary>
        public void update_14()
        {
            
            //最新，增加lasttime字段
            this.AddColumn<NewsModel>(1, 2, "lasttimeandid");
            this.AddColumn<CarSeriesQuoteModel>(2, 3, "GroupOrder");
            this.AddColumn<CarSeriesQuoteModel>(3, 4, "COrder");
            this.AddColumn<CarSeriesQuoteModel>(4, 5, "ParamIsShow");
            this.AddColumn<CarSeriesQuoteModel>(5, 6, "Compare");
            this.AddColumn<CarSeriesQuoteModel>(6, 7, "CompareText");

            this.AddTable<CarCompareModelT>(7, 8);

        }
    }
}

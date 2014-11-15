using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CommonLayer
{
    public class IsolatedStorageFileHelper
    {
        #region directory operation

        public static bool CreateDirectories(List<string> directoryPaths)
        {
            try
            {
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    foreach (string dir in directoryPaths)
                    {
                        if (!appStorage.DirectoryExists(dir))
                        {
                            appStorage.CreateDirectory(dir);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Exception in ReadIsoFile: {0}", ex.Message); 
                return false;
            }
        }

        #endregion

        #region string file operation

        public static string ReadIsoFile(string path)
        {
            string result = default(String);
            try
            {
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (appStorage.FileExists(path))
                    {
                        using (IsolatedStorageFileStream stream = appStorage.OpenFile(path, FileMode.Open, FileAccess.Read))
                        {
                            StreamReader sr = new StreamReader(stream);
                            result = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Exception in ReadIsoFile: {0}", ex.Message);
            }
            return result;
        }

        /// <summary>
        /// write string content to the specified path
        /// </summary>
        /// <param name="path">Complete path with file name.</param>
        /// <param name="content"></param>
        /// <param name="mode">FileMode used to open the file.</param>
        /// <returns>whether write correctly</returns>
        public static bool WriteIsoFile(string path, string content, FileMode mode)
        {
            try
            {
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = appStorage.OpenFile(path, mode, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(content);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Exception in WriteIsoFile: {0}", ex.Message);
                return false;
            }
        }

        public static bool DeleteIsoFile(string path)
        {
            try
            {
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (appStorage.FileExists(path))
                    {
                        appStorage.DeleteFile(path);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Exception in DeleteIsoFile: {0}", ex.Message);
                return false;
            }
        }

        #endregion

        #region image file operation

        public static bool SaveIsoImg(string path, BitmapImage source)
        {
            try
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source can't be null");
                }
                Image image = new Image();
                image.Source = source;
                WriteableBitmap wb = new WriteableBitmap(image, new TranslateTransform());
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream filestream = appStorage.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        Extensions.SaveJpeg(wb, filestream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Exception in SaveIsoImg: {0}", ex.Message);
                return false;
            }
        }

        public static BitmapImage ReadIsoImg(string path)
        {
            try
            {
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream filestream = appStorage.OpenFile(path, FileMode.Open, FileAccess.Read);
                    BitmapImage source = new BitmapImage();
                    source.SetSource(filestream);
                    return source;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Exception in ReadIsoImg: {0}", ex.Message);
                return null;
            }
        }

        #endregion
    }
}

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
using System.IO.IsolatedStorage;

namespace PD.Controls
{
    #region IsolatedStorageHelper
    /// <summary>
    /// 隔离存储辅助
    /// </summary>
    public static class IsolatedStorageHelper
    {
        private static readonly object lockStorageObject = new object();

        public static IsolatedStorageFile Storage { get {

            return IsolatedStorageFile.GetUserStoreForSite();
        } }

        #region 申请Silverlight隔离缓存空间
        /// <summary>
        /// 申请500M的存储空间
        /// </summary>
        /// <returns></returns>
        public static bool IncreaseQuotaTo()
        {
            //如果本函数代码断点调试将不能正常执行。申请缓存对话框将不能阻塞。
            long StorageSpace = 200 * 1024 * 1024;//申请空间大小
            long IncreaseSpace = 100 * 1024 * 1024;//增量空间
            long MinAvailableFreeSpace = 10 * 1024 * 1024;//最小可用空间
            try
            {
                lock (lockStorageObject)
                {
                    using (System.IO.IsolatedStorage.IsolatedStorageFile file = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForSite())
                    {
                        if (file.Quota < StorageSpace)
                        {
                            return file.IncreaseQuotaTo(StorageSpace);
                        }
                        else if (file.AvailableFreeSpace < MinAvailableFreeSpace)
                        {
                            return file.IncreaseQuotaTo(file.Quota + IncreaseSpace);
                        }
                    }
                }
                return true;
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException)
            {
                return false;
            }
        }
        #endregion

        #region WriteSlTextFile
        /// <summary>
        /// 保存信息至本地文件,SilverLight缓存中
        /// </summary>
        /// <param name="rFileName">存储文件名</param>
        /// <param name="rText">消息文本</param>
        public static void WriteSlTextFile(string rFileName, string rText)
        {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = null;
            System.IO.Stream stream = null;
            System.IO.TextWriter writer = null;
            try
            {
                isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                if (rFileName.IndexOf(':') >= 0)
                {
                    rFileName = rFileName.Substring(rFileName.LastIndexOf(':') + 1);
                }
                string rPath = System.IO.Path.GetDirectoryName(rFileName);
                if (isf.DirectoryExists(rPath) == false)
                {
                    isf.CreateDirectory(rPath);
                }
                stream = new System.IO.IsolatedStorage.IsolatedStorageFileStream(rFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, isf);
                //stream.Seek(0, System.IO.SeekOrigin.End);
                writer = new System.IO.StreamWriter(stream);
                writer.Write(rText);

                writer.Flush();
            }
            finally
            {
                try
                {
                    writer.Close();
                    stream.Close();
                    isf.Dispose();
                }
                catch
                {
                }
            }
        }
        #endregion

        #region ReadSlTextFile
        /// <summary>
        /// 读取本地文件信息,SilverLight缓存中
        /// </summary>
        /// <param name="rFileName">存储文件名</param>
        /// <returns>返回消息文本</returns>
        public static string ReadSlTextFile(string rFileName)
        {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = null;
            System.IO.Stream stream = null;
            System.IO.TextReader reader = null;
            try
            {
                isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                stream = new System.IO.IsolatedStorage.IsolatedStorageFileStream(rFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, isf);
                reader = new System.IO.StreamReader(stream);
                string sLine = reader.ReadToEnd();
                //string sLine=reader.ReadLine();
                return sLine;
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                try
                {
                    if (reader != null)
                    {
                        reader.Close(); // Close the reader
                    }
                    if (reader != null)
                    {
                        stream.Close();
                    }// Close the stream
                    isf.Dispose();
                }
                catch
                {

                }
            }
        }

        #endregion

        #region WriteSLStream

        /// <summary>
        /// 保存信息至本地文件,SilverLight缓存中
        /// </summary>
        /// <param name="rFileName"></param>
        /// <param name="buffer"></param>
        public static void WriteSlByteFile(string rFileName, byte[] buffer)
        {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = null;
            System.IO.Stream stream = null;
            try
            {
                isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

                if (rFileName.IndexOf(':') >= 0)
                {
                    rFileName = rFileName.Substring(rFileName.LastIndexOf(':') + 1);
                }
                ClearSlFile(rFileName);
                string rPath = System.IO.Path.GetDirectoryName(rFileName);
                if (rPath != "")
                {
                    isf.CreateDirectory(rPath);
                }

                stream = new System.IO.IsolatedStorage.IsolatedStorageFileStream(rFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.Write, isf);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
            finally
            {
                try
                {
                    stream.Close(); // Close the stream too
                    stream.Dispose();
                    isf.Dispose();
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// 读取本地文件信息,SilverLight缓存中
        /// </summary>
        /// <param name="rFileName">存储文件名</param>
        /// <returns>返回文件数据</returns>
        public static byte[] ReadSlByteFile(string rFileName)
        {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = null;
            System.IO.Stream stream = null;
            byte[] buffer = null;
            try
            {
                isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                if (!isf.FileExists(rFileName)) return null;
                stream = new System.IO.IsolatedStorage.IsolatedStorageFileStream(rFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, isf);
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close(); // Close the stream
                    stream.Dispose();
                }
                isf.Dispose();
            }
            return buffer;
        }
        #endregion

        #region ClearSlFile
        public static void ClearSlFile(string rFileName)
        {
            if (rFileName == null || rFileName == "") return;
            ClearSlFiles(rFileName);
        }

        public static void ClearSlAllFiles()
        {
            ClearSlFiles(null);
        }

        internal static void ClearSlFiles(string rFileName)
        {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            if (rFileName == null || rFileName.TrimEnd() == "")
            {
                isf.Remove();
            }
            else
            {
                if (isf.FileExists(rFileName))
                {
                    isf.DeleteFile(rFileName);
                }
            }
            isf.Dispose();
        }

        #endregion

        #region 创建文件夹
        public static void CreateDirectory(string path)
        {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();

    

            if (isf.DirectoryExists(path))
            {
                isf.CreateDirectory(path);
            }
        }
        public static string[] GetFilesName(string path) {
            System.IO.IsolatedStorage.IsolatedStorageFile isf = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            string searchpath = System.IO.Path.Combine(path, "*.*");

            string[] filesInSubDirs = isf.GetFileNames(searchpath);
            return filesInSubDirs;
        }

         
        #endregion
    }
    #endregion
}

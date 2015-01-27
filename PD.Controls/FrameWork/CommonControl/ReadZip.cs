
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
namespace PD.Controls.FrameWork.CommonControl
{
    public class ReadZip
    {
        public ElementStreamCollection Read(Stream stream)
        {
            ElementStreamCollection streams = new ElementStreamCollection();
            #region 读取压缩流

            ZipInputStream s = new ZipInputStream(stream);

            ZipEntry theEntry;

            while ((theEntry = s.GetNextEntry()) != null)
            {

                if (theEntry.CompressedSize == 0)
                    break;

                MemoryStream streamWriter = new MemoryStream();

                int size = 2048;

                byte[] source = new byte[2048];

                streams.Add(new ElementStream
                {
                    Stream = streamWriter,
                    ID = System.IO.Path.GetFileNameWithoutExtension(theEntry.Name)
                });

                while (true)
                {
                    size = s.Read(source, 0, source.Length);
                    if (size > 0)
                    {
                        streamWriter.Write(source, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }
                streamWriter.Seek(0, SeekOrigin.Begin);
            }

            s.Close();
            #endregion

            return streams;
        }
    }

    /// <summary>
    /// 元素流
    /// </summary>
    public class ElementStream
    {
        public string ID
        {
            get;
            set;
        }

        public Stream Stream
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 元素集合
    /// </summary>
    public class ElementStreamCollection : List<ElementStream>
    {
        public ElementStream this[string id]
        {
            get
            {
                return this.FirstOrDefault(item => item.ID == id);
            }
        }
    }
}

using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.IO;  
using System.IO.Compression;  
using System.Collections;  
using System.Runtime.Serialization.Formatters.Binary;  
using System.Runtime.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace UploadWcfService.Tools
{  
    /// <summary>  
   /// 压缩文件  
   /// </summary>  
  public  class ZipTools
    {
       #region 压缩解压单一文件  
       /// <summary>  
       /// 压缩单一文件  
       /// </summary>  
       /// <param name="sourceFile">源文件路径</param>  
       /// <param name="destinationFile">目标文件路径</param>  
       public static void CompressFile(string sourceFile, string destinationFile)  
       {  
           if (!File.Exists(sourceFile)) throw new FileNotFoundException();  
           using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))  
           {  
               byte[] buffer = new byte[sourceStream.Length];  
               int checkCounter = sourceStream.Read(buffer, 0, buffer.Length);  
               if (checkCounter != buffer.Length) throw new ApplicationException();  
               using (FileStream destinationStream = new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write))  
               {  
                   using (GZipStream compressedStream = new GZipStream(destinationStream, CompressionMode.Compress, true))  
                   {  
                       compressedStream.Write(buffer, 0, buffer.Length);  
                   }  
               } 
              
           }  
       }  

 
 
       /// <summary>  
       /// 解压缩文件  
       /// </summary>  
       /// <param name="sourceFile">源文件路径</param>  
       /// <param name="destinationFile">目标文件路径</param>  
       public static void DecompressFile(string sourceFile, string destinationFile)  
       {  
           if (!File.Exists(sourceFile)) throw new FileNotFoundException();  
           using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))  
           {  
               byte[] quartetBuffer = new byte[4];  
               int position = (int)sourceStream.Length - 4;  
               sourceStream.Position = position;  
               sourceStream.Read(quartetBuffer, 0, 4);  
               sourceStream.Position = 0;  
               int checkLength = BitConverter.ToInt32(quartetBuffer, 0);  
               byte[] buffer = new byte[checkLength + 100];  
               using (GZipStream decompressedStream = new GZipStream(sourceStream, CompressionMode.Decompress, true))  
               {  
                   int total = 0;  
                   for (int offset = 0; ; )  
                   {  
                       int bytesRead = decompressedStream.Read(buffer, offset, 100);  
                       if (bytesRead == 0) break;  
                       offset += bytesRead;  
                       total += bytesRead;  
                   }  
                   using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create))  
                   {  
                       destinationStream.Write(buffer, 0, total);  
                       destinationStream.Flush();  
                   }  
               }  
           }  
       }  

       #endregion  

       #region 压缩解压文件夹  
       /**/  
       /// <summary>  
       /// 对目标文件夹进行压缩，将压缩结果保存为指定文件  
       /// </summary>  
       /// <param name="dirPath">目标文件夹</param>  
       /// <param name="fileName">压缩文件</param>  
       public static void CompressFolder(string dirPath, string fileName)  
       {  
           ArrayList list = new ArrayList();  
            foreach (string f in Directory.GetFiles(dirPath))  
            {  
                byte[] destBuffer = File.ReadAllBytes(f);  
                SerializeFileInfo sfi = new SerializeFileInfo(f, destBuffer);  
                list.Add(sfi);  
            }  
            IFormatter formatter = new BinaryFormatter();  
            using (Stream s = new MemoryStream())  
            {  
                formatter.Serialize(s, list);  
                s.Position = 0;  
                CreateCompressFile(s, fileName);  
            }  
        }  
        /**/  
        /// <summary>  
        /// 对目标压缩文件解压缩，将内容解压缩到指定文件夹  
        /// </summary>  
        /// <param name="fileName">压缩文件</param>  
        /// <param name="dirPath">解压缩目录</param>  
        public static void DeCompressFolder(string fileName, string dirPath)  
        {  
            using (Stream source = File.OpenRead(fileName))  
            {  
                using (Stream destination = new MemoryStream())  
                {  
                    using (GZipStream input = new GZipStream(source, CompressionMode.Decompress, true))  
                    {  
                        byte[] bytes = new byte[4096];  
                        int n;  
                        while ((n = input.Read(bytes, 0, bytes.Length)) != 0)  
                        {  
                            destination.Write(bytes, 0, n);  
                        }  
                    }  
                    destination.Flush();  
                    destination.Position = 0;  
                    DeSerializeFiles(destination, dirPath);  
                }  
            }  
        }  
        private static void DeSerializeFiles(Stream s, string dirPath)  
        {  
            BinaryFormatter b = new BinaryFormatter();  
            ArrayList list = (ArrayList)b.Deserialize(s);  
            foreach (SerializeFileInfo f in list)  
            {  
                string newName = dirPath + Path.GetFileName(f.FileName);  
                using (FileStream fs = new FileStream(newName, FileMode.Create, FileAccess.Write))  
                {  
                    fs.Write(f.FileBuffer, 0, f.FileBuffer.Length);  
                    fs.Close();  
                }  
            }  
        }  
        private static void CreateCompressFile(Stream source, string destinationName)  
        {  
            using (Stream destination = new FileStream(destinationName, FileMode.Create, FileAccess.Write))  
            {  
                using (GZipStream output = new GZipStream(destination, CompressionMode.Compress))  
                {  
                    byte[] bytes = new byte[4096];  
                    int n;  
                    while ((n = source.Read(bytes, 0, bytes.Length)) != 0)  
                    {  
                        output.Write(bytes, 0, n);  
                    }  
                }  
            }  
        }





        /// <summary>
        /// ZIP:压缩单个文件
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="FileToZip">需要压缩的文件（绝对路径）</param>
        /// <param name="ZipedPath">压缩后的文件路径（绝对路径）</param>
        /// <param name="ZipedFileName">压缩后的文件名称（文件名，默认 同源文件同名）</param>
        /// <param name="CompressionLevel">压缩等级（0 无 - 9 最高，默认 5）</param>
        /// <param name="BlockSize">缓存大小（每次写入文件大小，默认 2048）</param>
        /// <param name="IsEncrypt">是否加密（默认 加密）</param>

        public static void ZipFile(string FileToZip, string ZipedPath, string ZipedFileName = "", int CompressionLevel = 5, int BlockSize = 2048, bool IsEncrypt = true)
        {
            //如果文件没有找到，则报错
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + FileToZip + " 不存在!");
            }

            //文件名称（默认同源文件名称相同）
            string ZipFileName = string.IsNullOrEmpty(ZipedFileName) ? ZipedPath + "\\" + new FileInfo(FileToZip).Name.Substring(0, new FileInfo(FileToZip).Name.LastIndexOf('.')) + ".zip" : ZipedPath + "\\" + ZipedFileName + ".zip";

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipFileName))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                {
                    using (System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        string fileName = FileToZip.Substring(FileToZip.LastIndexOf("\\") + 1);

                        ZipEntry ZipEntry = new ZipEntry(fileName);

                        if (IsEncrypt)
                        {
                            //压缩文件加密
                            ZipStream.Password = "123";
                        }

                        ZipStream.PutNextEntry(ZipEntry);

                        //设置压缩级别
                        ZipStream.SetLevel(CompressionLevel);

                        //缓存大小
                        byte[] buffer = new byte[BlockSize];

                        int sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                                ZipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }

                        StreamToZip.Close();
                    }

                    ZipStream.Finish();
                    ZipStream.Close();
                }

                ZipFile.Close();
            }
        }
        public static void CompressFile(byte[] source, string destinationFile)
        {
            using (FileStream destinationStream = new FileStream(destinationFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (GZipStream compressedStream = new GZipStream(destinationStream, CompressionMode.Compress, true))
                {
                    compressedStream.Write(source, 0, source.Length);
                }
            }
        } 


      /// <summary>
        /// ZIP：压缩文件夹
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="DirectoryToZip">需要压缩的文件夹（绝对路径）</param>
        /// <param name="ZipedPath">压缩后的文件路径（绝对路径）</param>
        /// <param name="ZipedFileName">压缩后的文件名称（文件名，默认 同源文件夹同名）</param>
        /// <param name="IsEncrypt">是否加密（默认 加密）</param>
        public static void ZipDirectory(string DirectoryToZip, string ZipedPath, string ZipedFileName = "", bool IsEncrypt = true)
        {
            //如果目录不存在，则报错
            if (!System.IO.Directory.Exists(DirectoryToZip))
            {
                throw new System.IO.FileNotFoundException("指定的目录: " + DirectoryToZip + " 不存在!");
            }

            //文件名称（默认同源文件名称相同）
            string ZipFileName = string.IsNullOrEmpty(ZipedFileName) ? ZipedPath + "\\" + new DirectoryInfo(DirectoryToZip).Name + ".zip" : ZipedPath + "\\" + ZipedFileName + ".zip";

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipFileName))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    if (IsEncrypt)
                    {
                        //压缩文件加密
                        s.Password = "123";
                    }
                    ZipSetp(DirectoryToZip, s, "");

                }
            }
        }
        /// <summary>
        /// 递归遍历目录
        /// add yuangang by 2016-06-13
        /// </summary>
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {

                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    ZipSetp(file, s, pPath);
                }

                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    using (FileStream fs = File.OpenRead(file))
                    {

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(fileName);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;

                        fs.Close();

                        crc.Reset();
                        crc.Update(buffer);

                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }


        /// <summary>
        /// ZIP:解压一个zip文件
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="ZipFile">需要解压的Zip文件（绝对路径）</param>
        /// <param name="TargetDirectory">解压到的目录</param>
        /// <param name="Password">解压密码</param>
        /// <param name="OverWrite">是否覆盖已存在的文件</param>
        public static void UnZip(string ZipFile, string TargetDirectory, string Password, bool OverWrite = true)
        {
            //如果解压到的目录不存在，则报错
            if (!System.IO.Directory.Exists(TargetDirectory))
            {
                throw new System.IO.FileNotFoundException("指定的目录: " + TargetDirectory + " 不存在!");
            }
            //目录结尾
            if (!TargetDirectory.EndsWith("\\")) { TargetDirectory = TargetDirectory + "\\"; }

            using (ZipInputStream zipfiles = new ZipInputStream(File.OpenRead(ZipFile)))
            {
                zipfiles.Password = Password;
                ZipEntry theEntry;

                while ((theEntry = zipfiles.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    string fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(TargetDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(TargetDirectory + directoryName + fileName) && OverWrite) || (!File.Exists(TargetDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(TargetDirectory + directoryName + fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zipfiles.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                zipfiles.Close();
            }
        }
 
        #endregion  
    }

  [Serializable]
  class SerializeFileInfo
  {
      public SerializeFileInfo(string name, byte[] buffer)
      {
          fileName = name;
          fileBuffer = buffer;
      }
      string fileName;
      public string FileName
      {
          get
          {
              return fileName;
          }
      }
      byte[] fileBuffer;
      public byte[] FileBuffer
      {
          get
          {
              return fileBuffer;
          }
      }
  }  

}

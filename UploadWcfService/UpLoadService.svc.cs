using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using UploadWcfService.Tools;
using UploadTools;
using System.Configuration;

namespace UploadWcfService
{
    // 注意: 如果更改此处的类名 "UpLoadService"，也必须更新 Web.config 中对 "UpLoadService" 的引用。
    public class UpLoadService : IUpLoadService
    {
        List<String> listFile = new List<string>();
        static List<String> listNewFile = new List<string>();
        private string uploadFolder = @"E:\WcfFileManage\";
        public List<String> GetNewFiles()
        {
            foreach (var item in listNewFile)
            {
                LogManage.WriteFile(uploadFolder, item);
            }
            
            return listNewFile;
        }

        
        private string fileFullPath = "";
        Stream myStream = null;
        static string folderPath = ConfigurationManager.AppSettings["folderPath"];
        public Stream DownLoadFile()
        {
            listNewFile = new List<string>();
            
            try
            {                
                string fileAll = LogManage.ReadFile(folderPath);
                DirectoryInfo TheFolder = new DirectoryInfo(folderPath);
              
                if (!Directory.Exists(folderPath + "FileTemp\\"))
                {
                    Directory.CreateDirectory(folderPath + "FileTemp\\");
                }
                foreach (FileInfo NextFile in TheFolder.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (NextFile.FullName.IndexOf("FileTemp.zip") > -1 || NextFile.FullName.IndexOf("FileTemp\\") > -1 || NextFile.FullName.IndexOf("HistoryFileList.txt") > -1)
                    {
                        continue;
                    }
                    string fullName = NextFile.FullName.Replace(folderPath, "");
                    if (fileAll.IndexOf(fullName + ",") == -1)
                    {
                        listNewFile.Add(fullName + ",");
                        string CopyFilePath = folderPath + "FileTemp\\" + fullName.Substring(0, fullName.LastIndexOf("\\") + 1);
                        if (!Directory.Exists(CopyFilePath))
                        {
                            Directory.CreateDirectory(CopyFilePath);
                        }
                        File.Copy(NextFile.FullName, CopyFilePath + NextFile.Name, true);
                    }
                    else
                    {

                    }
                }

                ZipTools.ZipDirectory(folderPath + "FileTemp\\", folderPath, "FileTemp", false);

               
                DirectoryInfo di = new DirectoryInfo(folderPath + "FileTemp");
                di.Delete(true);

                // string fileFullPath = Path.Combine(savePath, fileName);//服务器文件路径
                fileFullPath = folderPath + "FileTemp.zip";
                if (!File.Exists(fileFullPath))//判断文件是否存在
                {
                    return null;
                }

                //using (Stream fs = File.OpenRead(fileFullPath))
                //{
                //    return fs;
                //}
                myStream = File.OpenRead(fileFullPath);
                return myStream;
                //if (fileFullPath != "")
                //{
                //    File.Delete(fileFullPath);
                //}
                //File.Delete(fileFullPath);

                //StreamMessage sm = new StreamMessage();
                //sm.FileStream = myStream;
                //sm.listFile = listFile;
                //return sm;
            }
            catch (Exception ex)
            {
                LogManage.WriteSimpleLog(folderPath, ex.Message);
                return null;
            }
            finally
            {               
            }
        }

       

        public ReMessage UploadFile(FileUploadMessage request)
        {
            try
            {
               
                string savaPath = request.SavePath;
                string dateString = DateTime.Now.ToShortDateString() + @"\";
                string fileName = request.FileName;
                Stream sourceStream = request.FileData;
                FileStream targetStream = null;

                if (!sourceStream.CanRead)
                {
                    throw new Exception("数据流不可读!");
                }
                if (savaPath == null) savaPath = @"Photo\";
                if (!savaPath.EndsWith("\\")) savaPath += "\\";

                // uploadFolder = uploadFolder + savaPath + dateString;
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath = Path.Combine(uploadFolder, fileName);
                using (targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //read from the input stream in 4K chunks
                    //and save to output stream
                    const int bufferLen = 4096;
                    byte[] buffer = new byte[bufferLen];
                    int count = 0;
                    while ((count = sourceStream.Read(buffer, 0, bufferLen)) > 0)
                    {
                        targetStream.Write(buffer, 0, count);
                    }
                    targetStream.Close();
                    sourceStream.Close();
                }
                ZipTools.UnZip(filePath, uploadFolder, "");
                listFile = request.listFile;
                foreach (var item in listFile)
                {
                    LogManage.WriteFile(uploadFolder, item);
                }

                ReMessage reValue = new ReMessage();
                reValue.IsPass = true;
                return reValue;
            }
            catch (Exception ex)
            {
                LogManage.WriteSimpleLog(folderPath, ex.Message);
                ReMessage reValue = new ReMessage();
                reValue.IsPass = false;
                return reValue;
            }
        }

    }
}

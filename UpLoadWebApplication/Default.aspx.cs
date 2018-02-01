using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using UploadWcfService.Tools;
using Android.Widget;
using System.Collections.Generic;

namespace UpLoadWebApplication
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            channel = ser.ChannelFactory.CreateChannel();
        }
        UpLoadService.UpLoadServiceClient ser = new UpLoadWebApplication.UpLoadService.UpLoadServiceClient();
        UpLoadService.IUpLoadService channel;
        protected void Button1_Click(object sender, EventArgs e)
        {
           
            try
            {
                string folderPath = "E:\\SoftDownLoad\\";
                string fileAll = ReadFile(folderPath);
                DirectoryInfo TheFolder = new DirectoryInfo(folderPath);
                List<String> listFile = new List<string>();
                if (!Directory.Exists(folderPath + "FileTemp\\"))
                {
                    Directory.CreateDirectory(folderPath + "FileTemp\\");
                }
                foreach (FileInfo NextFile in TheFolder.GetFiles("*", SearchOption.AllDirectories))
                {
                    if (NextFile.FullName.IndexOf("FileTemp.zip") > -1 || NextFile.FullName.IndexOf("FileTemp\\") > -1)
                    {
                        continue;
                    }
                    string fullName = NextFile.FullName.Replace(folderPath, "");
                    if (fileAll.IndexOf(fullName + ",") == -1)
                    {
                        listFile.Add(fullName + ",");
                        string CopyFilePath = folderPath + "FileTemp\\" + fullName.Substring(0, fullName.LastIndexOf("\\")+1);
                        if (!Directory.Exists(CopyFilePath))
                        {
                            Directory.CreateDirectory(CopyFilePath);
                        }
                        File.Copy(NextFile.FullName, CopyFilePath + NextFile.Name,true);
                    }
                    else
                    {

                    }
                }



                ZipTools.ZipDirectory(folderPath + "FileTemp\\", folderPath , "FileTemp", false);

              
                //System.IO.Stream stream = FileUpload1.PostedFile.InputStream;
                //ser.UploadFile(FileUpload1.PostedFile.FileName, "pppp", stream);
                UpLoadService.FileUploadMessage request = new UpLoadService.FileUploadMessage();

                request.FileName = "FileTemp.zip";
                request.SavePath = "FileTemp";
                
                using (FileStream fs = File.OpenRead(folderPath + "FileTemp.zip"))
                {
                    request.FileData = fs;

                    request.listFile = listFile.ToArray();
                    UpLoadService.ReMessage reMessage = channel.UploadFile(request);
                    if (reMessage.IsPass == true)
                    {
                        foreach (var item in listFile)
                        {
                            WriteSimpleLog(folderPath, item);
                        }
                    }
                }
             
                File.Delete(folderPath + "FileTemp.zip");
                DirectoryInfo di = new DirectoryInfo(folderPath + "FileTemp");
                di.Delete(true);
               
            }
            catch (Exception ex)
            {

            }
          


            
        }

        public void WriteSimpleLog(string logFile, string FileName)
        {

            try
            {
                string logFilePath = logFile + "HistoryFileList.txt";
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath).Close();

                }
                System.IO.StreamWriter sw = System.IO.File.AppendText(logFilePath);

                sw.Write(FileName);

                sw.Close();
            }

            catch (Exception ex)
            {

            }

        }


        public string ReadFile(string FilePath)
        {

            try
            {
                string logFilePath = FilePath + "HistoryFileList.txt";

                System.IO.StreamReader sw = new StreamReader(logFilePath);

                string allfile = sw.ReadLine();

                sw.Close();
                return allfile;
            }

            catch (Exception ex)
            {
                return "";
            }

        }

        public Stream TransferDocument(string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return stream;
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "E:\\SoftDownLoad\\FileTemp.zip";
              

                Stream sourceStream = channel.DownLoadFile();
               // Stream sourceStream = sourceStreamMessage.FileStream;
                if (sourceStream != null)
                {
                    if (sourceStream.CanRead)
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            const int bufferLength = 4096;
                            byte[] myBuffer = new byte[bufferLength];
                            int count;
                            while ((count = sourceStream.Read(myBuffer, 0, bufferLength)) > 0)
                            {
                                fs.Write(myBuffer, 0, count);
                            }
                            fs.Close();
                            sourceStream.Close();
                        }

                        ZipTools.UnZip(filePath, "E:\\SoftDownLoad\\", "");

                        
                    }

                    string[] files = ser.GetNewFiles();
                    foreach (var item in files)
                    {
                        WriteSimpleLog("E:\\SoftDownLoad\\", item);
                    }
                }

            }
            catch (Exception ex)
            {

            }
           
        }
    }
}

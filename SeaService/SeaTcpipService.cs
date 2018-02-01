using ModubusTcpipService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UploadTools;
using UploadWcfService.Tools;

namespace CPMCS.SeaTcpipService
{
    public partial class SeaTcpipService : ServiceBase
    {
        static private Object objMonitor = new object();
        static private Object objControl = new object();
        System.Timers.Timer timer = new System.Timers.Timer();
        System.Timers.Timer timerOpenGate = new System.Timers.Timer();
        System.Timers.Timer timerDetection = new System.Timers.Timer();
        public ServiceHost _host;
        ModubusTcpipService.UpLoadService.UpLoadServiceClient ser = new ModubusTcpipService.UpLoadService.UpLoadServiceClient();
        ModubusTcpipService.UpLoadService.IUpLoadService channel;
        static string folderPath = ConfigurationManager.AppSettings["folderPath"];
        static int timerInterval = int.Parse(ConfigurationManager.AppSettings["TimerIntevalSecond"]);
        public SeaTcpipService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {

                channel = ser.ChannelFactory.CreateChannel();
                timer.Enabled = true;
                timer.Interval = timerInterval*1000;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

             

                
            }
            catch (Exception ex)
            {
               
            }
        }
     
        
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        { 
            try
            {

                timer.Enabled = false;
                LogManage.WriteSimpleLog(folderPath, folderPath);
                string fileAll = LogManage.ReadFile(folderPath);
               
                DirectoryInfo TheFolder = new DirectoryInfo(folderPath);
                List<String> listFile = new List<string>();
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
                        listFile.Add(fullName + ",");
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
               

                //System.IO.Stream stream = FileUpload1.PostedFile.InputStream;
                //ser.UploadFile(FileUpload1.PostedFile.FileName, "pppp", stream);
                ModubusTcpipService.UpLoadService.FileUploadMessage request = new ModubusTcpipService.UpLoadService.FileUploadMessage();

                request.FileName = "FileTemp.zip";
                request.SavePath = "FileTemp";

                using (FileStream fs = File.OpenRead(folderPath + "FileTemp.zip"))
                {
                    request.FileData = fs;


                    ModubusTcpipService.UpLoadService.ReMessage reMessage = channel.UploadFile(request);
                    if (reMessage.IsPass == true)
                    {
                        foreach (var item in listFile)
                        {
                            LogManage.WriteFile(folderPath, item);
                        }
                    }
                }

                File.Delete(folderPath + "FileTemp.zip");
                DirectoryInfo di = new DirectoryInfo(folderPath + "FileTemp");
                di.Delete(true);
                timer.Enabled = true;

            }
            catch (Exception ex)
            {
                timer.Enabled = true;
                LogManage.WriteSimpleLog(folderPath, ex.Message);
            }

            try
            {
                string filePath = folderPath+"FileTemp.zip";


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

                        ZipTools.UnZip(filePath, folderPath, "");


                    }

                    string[] files = ser.GetNewFiles();
                    foreach (var item in files)
                    {
                        LogManage.WriteFile(folderPath, item);
                    }
                }

            }
            catch (Exception ex)
            {
                timer.Enabled = true;
                LogManage.WriteSimpleLog(folderPath, ex.Message);
            }

        }


        protected override void OnStop()
        {
            try
            {
               
            }
            catch (Exception ex)
            { }
        }
    }
}

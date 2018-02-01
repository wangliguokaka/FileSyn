using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadTools
{
    public class LogManage
    {
        public static void WriteSimpleLog(string logFile, string msg)
        {

            try
            {
                string logFilePath = logFile + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath).Close();

                }
                System.IO.StreamWriter sw = System.IO.File.AppendText(logFilePath);

                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:  ") + msg);

                sw.Close();
            }

            catch (Exception ex)
            {

            }

        }

        public static string ReadFile(string FilePath)
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

        public static void WriteFile(string logFile, string FileName)
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
    }
}

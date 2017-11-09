using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProcurementErp.BLL
{
    public class Logger
    {
        private static int LogFileSize = int.Parse(ConfigurationManager.AppSettings["LOG_FILE_SIZE"]);
        public static void Log(string logDetails, string logType)
        {
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LOG_PATH"] + logType + "_log.txt"))
            {
                System.IO.FileInfo t = new System.IO.FileInfo(ConfigurationManager.AppSettings["LOG_PATH"] + logType + "_log.txt");
                if (t.Length > LogFileSize * 1024 * 1024)
                {
                    t.MoveTo(ConfigurationManager.AppSettings["LOG_PATH"] + logType + "_log_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt");
                }
            }
            System.IO.File.AppendAllText(ConfigurationManager.AppSettings["LOG_PATH"] + logType + "_log.txt", DateTime.Now.ToString() + " " + logDetails + Environment.NewLine);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Moldtrax.App_Start
{
    public static class Logger
    {
        public static void Log(string message)
        {
            try
            {
                string logFilePath =HttpContext.Current.Server.MapPath("~/logs/" + "Log.txt");
                //string logFilePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/" + dbName + "/") + "Log.txt";
                FileInfo logFileInfo = new FileInfo(logFilePath);
                DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
                {
                    using (StreamWriter log = new StreamWriter(fileStream))
                    {
                        string NewLine = "======================== " + System.DateTime.Now + " =======================================================================";
                        log.WriteLine(NewLine);
                        log.WriteLine(message);
                        log.Flush();
                        log.Close();
                    }
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
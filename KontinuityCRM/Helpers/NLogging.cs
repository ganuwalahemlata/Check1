using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using System.Runtime.CompilerServices;

namespace KontinuityCRM.Helpers
{
    public class NInLogger : INLogger
    {
        private Logger Logger = LogManager.GetCurrentClassLogger();

        public NInLogger()
        {
            //string assemblyFolder =@"O:\Ascertia\Products\New_Ascertia-Docs\AscertiaDocs_v6\trunk\AscertiaDocs\AscertiaDocs.Web.Presentation";
            string assemblyFolder = AppDomain.CurrentDomain.BaseDirectory;
            //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(assemblyFolder + "\\NLog.config", true);
        }
        public void LogInfo(string message, [CallerMemberName] string method = "", [CallerFilePath] string file = "", [CallerLineNumber] int lineNumber = 0)
        {
            var fileSplit = file.Split('\\');
            var fileName = fileSplit[fileSplit.Length - 1];
            Logger.Info("[{0}] - [{1}] - [{2}] - {3}", fileName, lineNumber, method, message);

            //Logger.Info(Message);
        }
        public void LogException(string message, Exception exception, string method = "", [CallerFilePath] string file = "", [CallerLineNumber] int lineNumber = 0)
        {
            var fileSplit = file.Split('\\');
            var fileName = fileSplit[fileSplit.Length - 1];
            Logger.Error("[{0}] - [{1}] - [{2}] - {3} -{4}", fileName, lineNumber, method,exception, message);

            //Logger.Info(Message);
        }
    }
}
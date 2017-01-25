using System;
using NLog;
using System.Runtime.CompilerServices;

namespace KontinuityCRM.Helpers
{
    public interface INLogger
    {
        void LogInfo(string message, [CallerMemberName] string method = "", [CallerFilePath] string file = "", [CallerLineNumber] int lineNumber = 0);
        void LogException(string message, Exception exception, string method = "", [CallerFilePath] string file = "", [CallerLineNumber] int lineNumber = 0);
    }
}

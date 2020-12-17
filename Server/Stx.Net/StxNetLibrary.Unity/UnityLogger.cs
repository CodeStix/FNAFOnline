using Stx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Stx.Utilities;
using UnityEngine;

namespace Stx.Net.Unity
{
    public class UnityLogger : ThreadSafeDataTransfer<UnityLogger.LoggedItem>, Logging.ILogger
    {
        public void Log(string message, LoggedImportance importance = LoggedImportance.Information, [CallerMemberName] string caller = "Global")
            => Received(new LoggedItem(message, importance, caller));
        
        public void LogException(Exception e, string note = null, [CallerMemberName] string caller = "Global")
            => Received(new LoggedItem((note == null ? "" : $"({ note }) ") + e.Message + "\n" + e.StackTrace, LoggedImportance.CriticalError, caller));
        
        protected override void Received(LoggedItem data)
        {
            data.Print();
        }

        public class LoggedItem
        {
            public string message;
            public LoggedImportance importance;
            public string caller;

            public static string NormalPrefix { get; set; } = "";
            public static string CriticalPrefix { get; set; } = "/ <b><color=#FF0000>Critical!</color></b>";
            public static string ErrorPrefix { get; set; } = "/ <color=#992211>Error</color>";
            public static string WarningPrefix { get; set; } = "/ <color=#999922>Warning</color>";
            public static string SuccessfulPrefix { get; set; } = "/ <i><color=#33FF00>Success!</color></i>";
            public static string DebugPrefix { get; set; } = "# <b><i><color=#9932CC>Debug</color></i></b> #";

            public static string PrintFormat { get; set;  }= "[<b><color=#BBBBBB>Stx</color><color=#DDDDDD>Net</color></b> $prefix] ($caller): $message";

            public LoggedItem(string message, LoggedImportance importance, string caller)
            {
                this.message = message;
                this.importance = importance;
                this.caller = caller;
            }

            public static string GetPrefixForImporance(LoggedImportance importance)
            {
                switch (importance)
                {
                    case LoggedImportance.CriticalError:
                        return CriticalPrefix;

                    case LoggedImportance.Warning:
                    case LoggedImportance.CriticalWarning:
                        return WarningPrefix;

                    case LoggedImportance.Debug:
                        return DebugPrefix;

                    case LoggedImportance.Successful:
                        return SuccessfulPrefix;

                    case LoggedImportance.Information:
                    default:
                        return NormalPrefix;
                }
            }

            public void Print()
            {
                string p = PrintFormat.Replace("$prefix", GetPrefixForImporance(importance))
                    .Replace("$caller", caller)
                    .Replace("$message", message);

                switch(importance)
                {
                    case LoggedImportance.Successful:
                    case LoggedImportance.Information:
                    case LoggedImportance.Debug:
                        Debug.Log(p);
                        return;

                    case LoggedImportance.Warning:
                    case LoggedImportance.CriticalWarning:
                        Debug.LogWarning(p);
                        return;

                    case LoggedImportance.CriticalError:
                        Debug.LogError(p);
                        return;
                }
            }
        }
    }
}

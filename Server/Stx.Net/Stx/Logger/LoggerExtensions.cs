using Stx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Logging
{
    public static class LoggerExtensions
    {
        public static void Debug(this ILogger logger, string message, [CallerMemberName] string caller = "Global")
        {
            logger.Log(message, LoggedImportance.Debug, caller);
        }

        public static void Info(this ILogger logger, string message, [CallerMemberName] string caller = "Global")
        {
            logger.Log(message, LoggedImportance.Information, caller);
        }

        public static void Error(this ILogger logger, string message, [CallerMemberName] string caller = "Global")
        {
            logger.Log(message, LoggedImportance.CriticalError, caller);
        }

        public static void CriticalError(this ILogger logger, string message, [CallerMemberName] string caller = "Global")
        {
            logger.Log(message, LoggedImportance.CriticalError, caller);
        }

        public static void Warning(this ILogger logger, string message, [CallerMemberName] string caller = "Global")
        {
            logger.Log(message, LoggedImportance.Warning, caller);
        }

        public static void CriticalWarning(this ILogger logger, string message, [CallerMemberName] string caller = "Global")
        {
            logger.Log(message, LoggedImportance.CriticalWarning, caller);
        }

        public static void Success(this ILogger logger, string message, [CallerMemberName] string caller = "Global")
        {
            logger.Log(message, LoggedImportance.Successful, caller);
        }

        public static void Error(this ILogger logger, Exception ex, string note = null, [CallerMemberName] string caller = "Global")
        {
            logger.LogException(ex, note, caller);
        }

        public static void Exception(this ILogger logger, Exception ex, string note = null, [CallerMemberName] string caller = "Global")
        {
            logger.LogException(ex, note, caller);
        }

        public static void CriticalError(this ILogger logger, Exception ex, string note = null, [CallerMemberName] string caller = "Global")
        {
            logger.LogException(ex, note, caller);
        }
    }
}

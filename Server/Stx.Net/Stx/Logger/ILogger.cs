using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Logging
{
    public interface ILogger
    {
        void Log(string message, LoggedImportance importance = LoggedImportance.Information, [CallerMemberName]string caller = "Global");
        void LogException(Exception ex, string note = null, [CallerMemberName]string caller = "Global");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Logging
{
    /// <summary>
    /// A logger that outputs to void, to nothing.
    /// </summary>
    public class VoidLogger : ILogger
    {
        public void Log(string message, LoggedImportance importance = LoggedImportance.Information, [CallerMemberName] string caller = "Global")
        { }

        public void LogException(Exception e, string note = null, [CallerMemberName] string caller = "Global")
        { }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Stx.Utilities.ErrorHandling
{
    [Obsolete]
    public class DebugHandler : ThreadSafeDataTransfer<DebuggedInfo>
    {
        public delegate void OnDebugDelegate(DebuggedInfo debugInfo);
        public static event OnDebugDelegate OnDebug;

        public delegate void OnInfoDelegate(DebuggedInfo infoDebugInfo);
        public static event OnInfoDelegate OnInfo;

        public delegate void OnWarningDelegate(DebuggedInfo warningDebugInfo);
        public static event OnWarningDelegate OnWarning;

        public delegate void OnErrorDelegate(CodedException error);
        public static event OnErrorDelegate OnError;

        public static DebugHandler Instance { get; set; } = new DebugHandler();

        public static bool AutoWriteToConsole { get; set; } = false;

        public DebugHandler() : base()
        {
        }

        public void LogInfo(string infoMessage, string caller = null)
        {
            Transfer(new DebuggedInfo(infoMessage, caller, DebugInfoType.Info));
        }

        public void LogWarning(string warningMessage, string caller = null)
        {
            Transfer(new DebuggedInfo(warningMessage, caller, DebugInfoType.Warning));
        }

        public void LogError(string errorMessage, int errorCode, string caller = null)
        {
            Transfer(new DebuggedInfo(new CodedException(errorMessage, errorCode), caller, DebugInfoType.Error));
        }

        public void LogError(CodedException error, string caller = null)
        {
            Transfer(new DebuggedInfo(error, caller, DebugInfoType.Error));
        }

        protected override void Received(DebuggedInfo data)
        {
            OnDebug?.Invoke(data);

            if (AutoWriteToConsole)
                WriteToConsole(data);

            if (data.Type == DebugInfoType.Info)
                OnInfo?.Invoke(data);
            else if(data.Type == DebugInfoType.Warning)
                OnWarning?.Invoke(data);
            else if (data.Type == DebugInfoType.Error)
                OnError?.Invoke(data.ErrorException);
        }

        public void WriteToConsole(DebuggedInfo debug)
        {
            var v = Console.ForegroundColor;

            if (debug.Type == DebugInfoType.Info)
                Console.ForegroundColor = ConsoleColor.Gray;
            else if (debug.Type == DebugInfoType.Warning)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (debug.Type == DebugInfoType.Error)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(debug.ToString());

            Console.ForegroundColor = v;
        }

        public static void Log(string infoMessage, string caller = null)
        {
            if (caller == null)
            {
                var st = new StackFrame(1).GetMethod();
                caller = $"{ st.DeclaringType.Name }/{ st.Name }"; ;
            }

            Instance.LogInfo(infoMessage, caller);
        }

        public static void Info(string infoMessage, string caller = null)
        {
            if (caller == null)
            {
                var st = new StackFrame(1).GetMethod();
                caller = $"{ st.DeclaringType.Name }/{ st.Name }"; ;
            }

            Instance.LogInfo(infoMessage, caller);
        }

        public static void Warning(string warningMessage, string caller = null)
        {
            if (caller == null)
            {
                var st = new StackFrame(1).GetMethod();
                caller = $"{ st.DeclaringType.Name }/{ st.Name }"; ;
            }

            Instance.LogWarning(warningMessage, caller);
        }

        public static void Error(string message, int errorCode = -1, string caller = null)
        {
            if (caller == null)
            {
                var st = new StackFrame(1).GetMethod();
                caller = $"{ st.DeclaringType.Name }/{ st.Name }"; ;
            }

            Instance.LogError(message, errorCode, caller);
        }

        public static void Error(Exception error, int errorCode = -1, string caller = null)
        {
            if (caller == null)
            {
                var st = new StackFrame(1).GetMethod();
                caller = $"{ st.DeclaringType.Name }/{ st.Name }"; ;
            }

            Instance.LogError(new CodedException(error, errorCode), caller);
        }

        public static void Error(CodedException error, string caller = null)
        {
            if (caller == null)
            {
                var st = new StackFrame(1).GetMethod();
                caller = $"{ st.DeclaringType.Name }/{ st.Name }"; ;
            }

            Instance.LogError(error, caller);
        }

        /*
        
        Error codes:

        Moved to
            F:\Documents\GitHub\Stx.Utilities.ErrorHandling.ErrorHandler Errors.txt

        */
    }

    [Obsolete]
    public class DebuggedInfo
    {
        public DebugInfoType Type { get; set; }
        public string Message { get; set; }
        public CodedException ErrorException { get; set; }
        public string CallerMethodName { get; set; }

        public static string ErrorStringFormat { get; set; } = "[{1}/Error] {0}";
        public static string WarningStringFormat { get; set; } = "[{1}/Warning] {0}";
        public static string InfoStringFormat { get; set; } = "[{1}/Info] {0}";

        public static string DefaultCallerString { get; set; } = "ANY";

        public DebuggedInfo(string message, string caller = null, DebugInfoType debugType = DebugInfoType.Info)
        {
            Type = debugType;
            Message = message;
            CallerMethodName = caller;
        }

        public DebuggedInfo(CodedException exception, string caller = null, DebugInfoType debugType = DebugInfoType.Error)
        {
            Type = debugType;
            Message = exception.ToString();
            ErrorException = exception;
            CallerMethodName = caller;
        }

        public string CallerToString()
        {
            if (CallerMethodName == null)
                return DefaultCallerString;
            else
                return CallerMethodName;
        }

        public override string ToString()
        {
            if (Type == DebugInfoType.Info)
            {
                return string.Format(InfoStringFormat, Message, CallerToString());
            }
            else if (Type == DebugInfoType.Warning)
            {
                return string.Format(WarningStringFormat, Message, CallerToString());
            }
            else if (Type == DebugInfoType.Error)
            {
                if (ErrorException != null)
                    return ErrorException.ToString();
                else
                    return string.Format(ErrorStringFormat, Message, CallerToString());
            }
            else
                return base.ToString();
        }
    }

    public enum DebugInfoType
    {
        Info,
        Warning,
        Error
    }

    /*public class DebugHandler : ThreadSafeDataTransfer<CodedException>
    {
        public delegate void OnErrorDelegate(CodedException error);
        public event OnErrorDelegate OnError;

        public static DebugHandler Instance = new DebugHandler();

        public DebugHandler() : base()
        {
        }

        public void Error(Exception e, int errorCode)
        {
            //OnError?.Invoke(new CodedException(e,errorCode));
            Transfer(new CodedException(e, errorCode));
        }

        public void Error(string errorMessage, int errorCode)
        {
           // OnError?.Invoke(new CodedException(errorMessage, errorCode));
            Transfer(new CodedException(errorMessage, errorCode));
        }

        protected override void Received(CodedException data)
        {
            //Console.WriteLine(data.ToString());
            OnError?.Invoke(data);
        }


    }*/

    public class CodedException
    {
        public Exception Error { get; }
        public int ErrorCode { get; }

        public static string ErrorStringFormat { get; set; } = "Error #{0}:: {1}\n\t{2}\n\t{3}";

        public CodedException(Exception exception, int errorCode)
        {
            this.Error = exception;
            this.ErrorCode = errorCode;
        }

        public CodedException(string message, int errorCode)
        {
            this.Error = new Exception(message);
            this.ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return string.Format(ErrorStringFormat, ErrorCode, Error.Message, Error.StackTrace, Error.InnerException?.Message);
        }
    }

    /*public interface IThreadSafeDataTransfer<T>
    {
        bool InvokeEventsOnReceived { get; set; }
        Queue<T> DataTransfer { get; }

        void Transfer(T data);
        void InvokeEvents();
        void Received(T data);
    }*/

    
}



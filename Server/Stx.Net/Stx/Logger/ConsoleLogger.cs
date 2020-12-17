using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stx.Logging
{
    /// <summary>
    /// A logger that outputs to the console window.
    /// </summary>
    public class ConsoleLogger : ILogger, IDisposable
    {
        public bool WriteAsync { get; } = true;

        private Thread writingThread;
        private ConcurrentQueue<Logged> toWrite = new ConcurrentQueue<Logged>();
        private ManualResetEvent toWriteAvailable = new ManualResetEvent(false);

        /// <summary>
        /// A logger that outputs to the console window.
        /// </summary>
        /// <param name="writeAsync">Should this logger be writing to the console on a separate thread and not cause blocking?</param>
        public ConsoleLogger(bool writeAsync = true)
        {
            WriteAsync = writeAsync;

            writingThread = new Thread(new ThreadStart(AsyncLogger));
            writingThread.Start();
        }

        private void Logger(string message, LoggedImportance importance = LoggedImportance.Information, string caller = "Global")
        {
            if (WriteAsync)
            {
                toWrite.Enqueue(new Logged(message, importance, caller));
                toWriteAvailable.Set();
            }
            else
            {
                SyncLogger(message, importance, caller);
            }
        }

        private void AsyncLogger()
        {
            while (true)
            {
                toWriteAvailable.WaitOne();

                while (toWrite.Count > 0)
                {
                    Logged d;
                    if (toWrite.TryDequeue(out d))
                    {
                        SyncLogger(d.message, d.importance, d.caller);
                    }
                }

                toWriteAvailable.Reset();
            }
        }

        private struct Logged
        {
            public string message;
            public LoggedImportance importance;
            public string caller;

            public Logged(string message, LoggedImportance importance, string caller)
            {
                this.message = message;
                this.importance = importance;
                this.caller = caller;
            }
        }

        private void SyncLogger(string message, LoggedImportance importance = LoggedImportance.Information, string caller = "Global")
        {
            ConsoleColor primaryForeColor = ConsoleColor.White;
            ConsoleColor secondaryForeColor = ConsoleColor.Gray;
            ConsoleColor backColor = ConsoleColor.Black;

            switch (importance)
            {
                case LoggedImportance.CriticalError:
                    backColor = ConsoleColor.DarkRed;
                    primaryForeColor = ConsoleColor.Red;
                    break;

                case LoggedImportance.CriticalWarning:
                    backColor = ConsoleColor.DarkYellow;
                    primaryForeColor = ConsoleColor.Yellow;
                    secondaryForeColor = ConsoleColor.Black;
                    break;

                case LoggedImportance.Warning:
                    primaryForeColor = ConsoleColor.Yellow;
                    secondaryForeColor = ConsoleColor.DarkYellow;
                    break;

                case LoggedImportance.Information:
                    secondaryForeColor = ConsoleColor.DarkCyan;
                    primaryForeColor = ConsoleColor.Gray;
                    break;

                case LoggedImportance.Successful:
                    primaryForeColor = ConsoleColor.Green;
                    secondaryForeColor = ConsoleColor.DarkGreen;
                    break;

                case LoggedImportance.Debug:
                    primaryForeColor = ConsoleColor.Magenta;
                    secondaryForeColor = ConsoleColor.DarkMagenta;
                    break;
            }

            Console.BackgroundColor = backColor;
            Console.ForegroundColor = secondaryForeColor;
            Console.Write($"[{ DateTime.Now.ToLongTimeString() }] { caller }: ");
            Console.ForegroundColor = primaryForeColor;
            FormattedWriteLine(message);
            Console.ResetColor();
        }

        private void FormattedWriteLine(string str)
        {
            FormattedWrite(str);
            Console.WriteLine();
        }

        private void FormattedWrite(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '&' && i != str.Length - 1)
                {
                    char c = str[i + 1];
                    int cc = -1;
                    if (c == 'a')
                        cc = 10;
                    if (c == 'b')
                        cc = 11;
                    if (c == 'c')
                        cc = 12;
                    if (c == 'd')
                        cc = 13;
                    if (c == 'e')
                        cc = 14;
                    if (c == 'f')
                        cc = 15;
                    if (c == 'r')
                        cc = 16;
                    if (cc == -1)
                        if (!int.TryParse(c.ToString(), out cc))
                            continue;
                    if (cc != 16)
                        Console.ForegroundColor = (ConsoleColor)cc;
                    else
                        Console.ResetColor();
                    i++;
                }
                else
                {
                    Console.Write(str[i]);
                }
            }
        }

        public void Log(string message, LoggedImportance importance = LoggedImportance.Information, [CallerMemberName] string caller = "Global")
            => Logger(message, importance, caller);

        public void LogException(Exception e, string note = null, [CallerMemberName] string caller = "Global")
            => Logger((note == null ? "" : $"({ note }) ") + e.Message + "\n" + e.StackTrace, LoggedImportance.CriticalError, caller);

        public void Dispose()
        {
            writingThread.Abort();

            toWriteAvailable.Dispose();
        }
    }
}

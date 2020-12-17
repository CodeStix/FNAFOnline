using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stx.Utilities
{
    public abstract class ThreadSafeDataTransfer<T> : IDataChecker
    {
        /// <summary>
        /// When true:
        ///     Thread 1: Transfers, use <see cref="Transfer(T)"/>;
        ///     Thread 2: Receives, use <see cref="Received(T)"/>;
        /// </summary>
        public bool MultiThread { get; set; } = true;

        //public delegate void OnReceivedDelegate(T data);
        //public event OnReceivedDelegate OnReceivedData;

        protected volatile int dataAvailable = 0;
        protected ConcurrentQueue<T> DataTransfer { get; set; }

        public ThreadSafeDataTransfer(bool addToGlobalChecker = true)
        { 
            if (addToGlobalChecker)
                ThreadSafeData.AddDataChecker(this);

            DataTransfer = new ConcurrentQueue<T>();
        }

        protected void Transfer(T data)
        {
            DataTransfer.Enqueue(data);
            dataAvailable++;

            if (!IsMultiThread())
                CheckForData();
        }

        public void CheckForData()
        {
            while (dataAvailable > 0)
            {
                T d;
                if (!DataTransfer.TryDequeue(out d))
                    break;

                dataAvailable--;

                Received(d);
                //OnReceivedData?.Invoke(d);
            }
        }

        public bool IsMultiThread()
        {
            if (!ThreadSafeData.MultiThreadOverride.HasValue)
                return MultiThread;
            else
                return ThreadSafeData.MultiThreadOverride.Value;
        }

        protected abstract void Received(T data);
    }

    /// <summary>
    /// Provides thread safe data transfers and event invokes.
    /// <para>To transfer data in a class, the class should implement <see cref="ThreadSafeDataTransfer{T}"/> where T is the type that is being transfered. 
    /// Use <see cref="ThreadSafeDataTransfer{T}.CheckForData()"/> to transfer the data thread-safely.</para>
    /// Use <see cref="ThreadSafeData.CheckForAllData()"/> to transfer all data on every class that implements <see cref="ThreadSafeDataTransfer{T}"/>.
    /// <para>To invoke an event, use <see cref="ThreadSafeData.Invoke(Action)"/>. 
    /// And use <see cref="ThreadSafeData.InvokeAllEvents()"/> to invoke all the pending events thread-safely.</para>
    /// </summary>
    public static class ThreadSafeData
    {
        public static bool? MultiThreadOverride { get; set; } = null;
        public static ConcurrentDictionary<int, IDataChecker> DataCheckers { get; set; } = new ConcurrentDictionary<int, IDataChecker>();

        public static ThreadSafeEvents Events { get; } = new ThreadSafeEvents();

        private static int registerIndex = 0;

        public static void AddDataChecker(IDataChecker dataChecker)
        {
            DataCheckers.TryAdd(registerIndex++, dataChecker);
        }

        public static void CheckForAllData()
        {
            foreach (var a in DataCheckers.Values)
                a.CheckForData();

            InvokeAllEvents();
        }

        /// <summary>
        /// Invokes all pending events on the current thread.
        /// </summary>
        public static void InvokeAllEvents()
        {
            Events.CheckForData();
        }

        /// <summary>
        /// Invokes a event thread-safely, call it using <see cref="InvokeAllEvents()"/>.
        /// </summary>
        /// <param name="a"></param>
        public static void Invoke(Action a)
        {
            Events.DoInvoke(a);
        }
    }

    public class ThreadSafeEvents : ThreadSafeDataTransfer<Action>
    {
        protected override void Received(Action data)
        {
            data.Invoke();
        }

        public void DoInvoke(Action a)
        {
            Transfer(a);
        }
    }

    public interface IDataChecker
    {
        void CheckForData();
    }
}


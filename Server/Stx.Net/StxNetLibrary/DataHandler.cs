using Stx.Logging;
using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using Stx.Collections.Concurrent;

namespace Stx.Net
{
    public class AnyDataHandler
    {
        public string requiredKey;
    }

    public class DataHandler<T> : AnyDataHandler
    {
        public delegate void ReceivedDelegate(T value, IDataHolder p);
        public event ReceivedDelegate Received;

        /// <summary>
        /// Creates a new <see cref="DataHandler{T}"/> object with the required key to invoke the events.
        /// </summary>
        /// <param name="requiredKey">The <see cref="Hashtable"/> key to associate with the <see cref="Received"/> event.</param>
        /// <param name="onReceive">The first delegate to be added to the <see cref="Received"/> event. Can be null to not add it.</param>
        public DataHandler(string requiredKey, ReceivedDelegate onReceive)
        {
            this.requiredKey = requiredKey;
            if (onReceive != null)
                this.Received += onReceive;
        }

        public void InvokeEvent(T value, IDataHolder p)
        {
            Received?.Invoke(value, p);
        }
    }

    public class ObjectHandler : DataHandler<object>
    {
        /// <summary>
        /// Creates a new <see cref="ObjectHandler"/> object with the required key to invoke the events.
        /// This data handler accepts all incoming objects.
        /// </summary>
        /// <param name="requiredKey">The <see cref="Hashtable"/> key to associate with the <paramref name="onReceive"/> delegate.</param>
        /// <param name="onReceive">The first delegate to be added to the <paramref name="onReceive"/> delegate. Can be null to not add it.</param>
        public ObjectHandler(string requiredKey, ReceivedDelegate onReceive) : base(requiredKey, onReceive)
        {
        }
    }

    public class DataReceiver : ThreadSafeDataTransfer<IDataHolder>
    {
        public ConcurrentList<AnyDataHandler> handlers = new ConcurrentList<AnyDataHandler>();

        public static ILogger Logger { get; set; } = StxNet.DefaultLogger;

        /// <summary>
        /// Adds a handler to this <see cref="DataReceiver"/> object.
        /// </summary>
        /// <param name="handler">The handler to add, this should be either a <see cref="DataHandler{T}"/> or <see cref="ObjectHandler"/> object.</param>
        public void AddHandler(AnyDataHandler handler)
        {
            if (!handlers.Contains(handler))
                handlers.Add(handler);
        }

        public void RemoveHandler(string withKey)
        {
            lock(handlers.Locker)
            {
                var v = handlers.Where((e) => e.requiredKey == withKey);

                foreach (var vv in v)
                    handlers.Remove(vv);
            }
        }

        /// <summary>
        /// Pass all the received data to this method to: call the associated events/enqueue them to call the events later. See <see cref="ThreadSafeData"/>.
        /// </summary>
        /// <param name="p">The received data to handle.</param>
        public void DidReceive(IDataHolder p)
        {
            Transfer(p);
        }

        protected override void Received(IDataHolder p)
        {
            foreach (AnyDataHandler h in handlers.Where((e) => p.Data.ContainsKey(e.requiredKey)))
            {
                object value = p.Data[h.requiredKey];

                if (h is ObjectHandler)
                {
                    ((ObjectHandler)h).InvokeEvent(value, p);
                }
                else
                {
                    Type t = h.GetType();

                    if (!t.IsGenericType)
                        continue;

                    Type firstGeneric = t.GetGenericArguments().FirstOrDefault();

                    if (firstGeneric == null)
                        continue;

                    object[] par = new object[2];

                    if (value != null && firstGeneric.IsAssignableFrom(value.GetType()))
                        par[0] = Convert.ChangeType(value, firstGeneric);
                    else
                        par[0] = null;

                    par[1] = p;

                    //DebugHandler.Info($"Invoking handled event for { h.requiredKey }; data type " + firstGeneric.FullName + " = " + par[0]);

                    MethodInfo method = t.GetMethod("InvokeEvent");
                    try
                    {
                        method.Invoke(h, par);
                    }
                    catch
                    {
                        Logger.Log("Could not invoke handling event method. ", LoggedImportance.CriticalWarning);
                    }
                }
            }
        }
    }
}

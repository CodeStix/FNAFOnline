using Stx.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using Stx.Logging;

namespace Stx.Net
{
    public class Packet : IDataHolder, IByteDefined<Packet>, ISerializationEvent
    {
        public BHashtable Data { get; set; } = new BHashtable();
        public string SenderID { get; protected set; }
        public DateTime CreationTime { get; internal set; }

        public Packet()
        { }

        public Packet(string senderID)
        {
            Data = new BHashtable();
            SenderID = senderID;
            CreationTime = DateTime.UtcNow;
        }

        public Packet(Hashtable data, string senderID)
        {
            Data = new BHashtable(data);
            SenderID = senderID;
            CreationTime = DateTime.UtcNow;
        }

        public byte[] ToBytes()
        {
            return Bytifier.Bytes(this);
        }

        public override string ToString()
        {
            List<string> content = new List<string>();

            foreach (string key in Data)
                content.Add($"{ key }: { Data[key] }");

            return $"({ (string.Join(", ", content)) })";
        }

        public static Packet SingleDataPacket(string senderID, string key, object value)
        {
            Packet p = new Packet(senderID);
            p.Data.Add(key, value);
            return p;
        }
         
        public virtual void BeforeSerialize()
        { }

        public virtual void AfterDeserialize()
        { }
    }

    public class RequestPacket : Packet, IByteDefined<RequestPacket>, ISerializationEvent
    {
        public class Completion : IByteDefined<Completion>
        {
            public string RequiredKey { get; set; }
            public string RequiredKeyTypeName { get; set; }

            public Completion()
            { }

            public Completion(string requiredKey, Type requiredKeyType)
            {
                RequiredKey = requiredKey;
                RequiredKeyTypeName = requiredKeyType.FullName;
            }
        }

        /*public class PacketCompletion : IByteDefined<PacketCompletion>
        {
            public BHashtable RequiredKeysTypes { get; set; }

            public PacketCompletion()
            {
                RequiredKeysTypes = new BHashtable();
            }

            public void StillRequires<T>(string key)
            {
                RequiredKeysTypes.Add(key, typeof(T).FullName);
            }

            public void StillRequires<T0, T1>(string key0, string key1)
            {
                RequiredKeysTypes.Add(key0, typeof(T0).FullName);
                RequiredKeysTypes.Add(key1, typeof(T1).FullName);
            }

            public void StillRequires<T0, T1, T2>(string key0, string key1, string key2)
            {
                RequiredKeysTypes.Add(key0, typeof(T0).FullName);
                RequiredKeysTypes.Add(key1, typeof(T1).FullName);
                RequiredKeysTypes.Add(key2, typeof(T2).FullName);
            }

            public void StillRequires<T0, T1, T2, T3>(string key0, string key1, string key2, string key3)
            {
                RequiredKeysTypes.Add(key0, typeof(T0).FullName);
                RequiredKeysTypes.Add(key1, typeof(T1).FullName);
                RequiredKeysTypes.Add(key2, typeof(T2).FullName);
                RequiredKeysTypes.Add(key3, typeof(T3).FullName);
            }
        }*/

        public delegate void RequestResponseDelegate(object requested); // Only used in RequestPacket class.

        /// <summary>
        /// Gets the name of the item this packet requests.
        /// </summary>
        public string RequestItemName { get; protected set; }
        /// <summary>
        /// The ID of this request.
        /// </summary>
        public string RequestID { get; protected set; }
        /// <summary>
        /// The response to the request.
        /// </summary>
        public object ResponseObject { get; set; } = null;
        /// <summary>
        /// The time it took for the packet to reach the server, get responded and come back.
        /// </summary>
        [DoNotSerialize]
        public TimeSpan SendReceiveTime { get; private set; } = TimeSpan.MaxValue;
        /// <summary>
        /// The time when this packet was received.
        /// </summary>
        [DoNotSerialize]
        public DateTime ReceivedTime { get; private set; }
        /// <summary>
        /// Should we answer the packet and resend it?
        /// </summary>
        [DoNotSerialize]
        public bool ShouldRespond { get; set; } = true;

        /// <summary>
        /// The status of the response.
        /// </summary>
        [DoNotSerialize]
        public PacketResponseStatus Status
        {
            get
            {
                string str = ResponseObject?.ToString();

                if (str == RequestResponseFailed)
                    return PacketResponseStatus.Failed;
                else if (str == RequestResponseOkey)
                    return PacketResponseStatus.Okey;
                else if (str == RequestResponseUnknown)
                    return PacketResponseStatus.UnknownRequest;
                else if (ResponseObject is Completion)
                    return PacketResponseStatus.RequiresCompletion;
                else
                    return PacketResponseStatus.Responded;
            }
        }

        public static Dictionary<string, RequestResponseDelegate> requests; // Not used by server
        public static Dictionary<string, string> requestItems;

        /// <summary>
        /// The time the last request packet took to reach the server, get responded and come back.
        /// </summary>
        [DoNotSerialize]
        public static TimeSpan LastSendReceiveTime { get; private set; } = TimeSpan.MaxValue;

        public const string RequestResponseUnknown = "UNKNOWN";
        public const string RequestResponseOkey = "OKEY";
        public const string RequestResponseFailed = "FAILED";

        [DoNotSerialize]
        public static ILogger Logger { get; set; } = StxNet.DefaultLogger;

        public RequestPacket()
        { }

        public RequestPacket(string senderID, string requestItemName, RequestResponseDelegate onResponse) : base(senderID)
        {
            RequestItemName = requestItemName.Trim();
            RequestID = Guid.NewGuid().ToString();

            if (requests == null)
                requests = new Dictionary<string, RequestResponseDelegate>();
            if (requestItems == null)
                requestItems = new Dictionary<string, string>();

            if (!requests.ContainsKey(RequestID))
            {
                requests.Add(RequestID, onResponse);
                requestItems.Add(RequestID, RequestItemName);
            }
            else
            {
                Logger.Log("A packet with duplicate request ID was created, the chance of this happening is VERY VERY very small. You are very lucky or unlucky.", LoggedImportance.CriticalWarning);
            }
        }

        /// <summary>
        /// Respond to the request by sending the asked object, make sure you send this packet back.
        /// This request's status will be marked as <see cref="PacketResponseStatus.Responded"/>.
        /// </summary>
        /// <param name="respondObject">The object to respond the request with.</param>
        public void Respond(object respondObject)
        {
            ResponseObject = respondObject;
            Data.Clear();
        }

        /// <summary>
        /// Responds to the request with a <see cref="PacketResponseStatus.Failed"/> status.
        /// </summary>
        public void ResponseFail()
        {
            ResponseObject = RequestResponseFailed;
        }

        /// <summary>
        /// Responds to the request with a <see cref="PacketResponseStatus.UnknownRequest"/> status.
        /// </summary>
        public void ResponseUnknown()
        {
            ResponseObject = RequestResponseUnknown;
        }

        /// <summary>
        /// Responds to the request with a <see cref="PacketResponseStatus.Okey"/> status.
        /// </summary>
        public void ResponseOk()
        {
            ResponseObject = RequestResponseOkey;
        }

        /// <summary>
        /// Responds to the request with a <see cref="PacketResponseStatus.RequiresCompletion"/> status.
        /// </summary>
        /// <typeparam name="T">The required type of the still required key.</typeparam>
        /// <param name="key">The still required key.</param>
        public void ResponseRequires<T>(string key)
        {
            ResponseObject = new Completion(key, typeof(T));
        }

        /// <summary>
        /// Do not respond to this packet. Sets <see cref="ShouldRespond"/> to false.
        /// </summary>
        public void DoNotRespond()
        {
            ShouldRespond = false;
        }

        /// <summary>
        /// Did this packet get responded?
        /// </summary>
        /// <returns>The fact that the packet was responded.</returns>
        public bool DidRespond()
        {
            return ResponseObject != null;
        }

        /// <summary>
        /// Do not touch this method, you will cause problems!
        /// Used by client to invoke methods that are associated with their request.
        /// </summary>
        public void InvokeResponseMethods()
        {
            if (requests.ContainsKey(RequestID))
            {
                requests[RequestID]?.Invoke(ResponseObject);
                requests.Remove(RequestID);

                requestItems.Remove(RequestID);
            }
            else
            {
                Logger.Log($"There was not a delegate available for request: { RequestItemName }({ RequestID }) Skipping...", LoggedImportance.CriticalWarning);
            }
        }

        public void SetResponderID(string responderID)
        {
            this.SenderID = responderID;
        }

        /// <summary>
        /// Convert raw answer to a <see cref="ServerResponse{T}"/> delegate that contains more information.
        /// </summary>
        /// <typeparam name="T">The item to cast to if the packet's status is <see cref="PacketResponseStatus.Responded"/></typeparam>
        /// <param name="receivedResponse"></param>
        /// <param name="delegateToInvoke"></param>
        public static void RequestPacketStatus<T>(object receivedResponse, ServerResponse<T> delegateToInvoke)
        {
            if (delegateToInvoke == null)
                return;

            string rawResponse = receivedResponse?.ToString();

            if (rawResponse == RequestResponseFailed)
                delegateToInvoke.Invoke(PacketResponseStatus.Failed, default(T));
            else if (rawResponse == RequestResponseOkey)
                delegateToInvoke.Invoke(PacketResponseStatus.Okey, default(T));
            else if (rawResponse == RequestResponseUnknown)
                delegateToInvoke.Invoke(PacketResponseStatus.UnknownRequest, default(T));
            else if (receivedResponse is Completion)
                delegateToInvoke.Invoke(PacketResponseStatus.RequiresCompletion, default(T));
            else
                delegateToInvoke.Invoke(PacketResponseStatus.Responded, (T)receivedResponse);
        }

        public static void RequestPacketStatus(object receivedResponse, ServerResponse delegateToInvoke)
        {
            if (delegateToInvoke == null)
                return;

            string rawResponse = receivedResponse?.ToString();

            if (rawResponse == RequestResponseFailed)
                delegateToInvoke.Invoke(PacketResponseStatus.Failed);
            else if (rawResponse == RequestResponseOkey)
                delegateToInvoke.Invoke(PacketResponseStatus.Okey);
            else if (rawResponse == RequestResponseUnknown)
                delegateToInvoke.Invoke(PacketResponseStatus.UnknownRequest);
            else if (receivedResponse is Completion)
                delegateToInvoke.Invoke(PacketResponseStatus.RequiresCompletion);
            else
                delegateToInvoke.Invoke(PacketResponseStatus.Responded);
        }

        public override string ToString()
        {
            List<string> content = new List<string>();

            content.Add($"RequestedItem: { RequestItemName }");

            foreach (string key in Data)
                content.Add($"{ key }: { Data[key] }");

            if (ResponseObject != null)
                content.Add("Response: " + ResponseObject.ToString());

            return $"({ (string.Join(", ", content)) })";
        }

        public new byte[] ToBytes()
        {
            return Bytifier.Bytes(this);
        }

        public override void AfterDeserialize()
        {
            base.AfterDeserialize();

            ReceivedTime = DateTime.UtcNow;
            SendReceiveTime = ReceivedTime - CreationTime;
            LastSendReceiveTime = SendReceiveTime;
        }
    }

}

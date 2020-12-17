using Stx.Serialization;
using System.Collections;
using System.Threading.Tasks;

namespace Stx.Net
{
    public delegate void ServerResponse<T>(PacketResponseStatus status, T response);
    public delegate void ServerResponse(PacketResponseStatus status);

    public class ServerRequester
    {
        public IServerConnected Connection { get; private set; }

        public ServerRequester(IServerConnected connection)
        {
            Connection = connection;
        }

        public Task<T> Request<T>(string requestedItemName, Hashtable extraData = null)
        {
            return Task.Run(async () =>
            {
                bool responded = false;
                T r = default(T);

                RequestAsync<T>(requestedItemName, (state, rr) =>
                {
                    r = rr;
                    responded = true;

                }, extraData);

                while (!responded)
                    await Task.Yield();

                return r;
            });
        }

        public void RequestAsync(string requestedItemName, Hashtable extraData = null)
        {
            RequestAsync<object>(requestedItemName, (state, e) => { }, extraData);
        }

        public void RequestAsync<T>(string requestedItemName, ServerResponse<T> response, Hashtable extraData = null)
        {
            RequestPacket p = Connection.GetNewRequestPacket(requestedItemName, (obj) => RequestPacket.RequestPacketStatus(obj, response));

            if (extraData != null)
                p.Data = new BHashtable(extraData);

            Connection.SendToServer(p.ToBytes(), BytesContentType.Packet);
        }

        public void RequestAsync(string requestedItemName, ServerResponse response, Hashtable extraData = null)
        {
            RequestPacket p = Connection.GetNewRequestPacket(requestedItemName, (obj) => RequestPacket.RequestPacketStatus(obj, response));

            if (extraData != null)
                p.Data = new BHashtable(extraData);

            Connection.SendToServer(p.ToBytes(), BytesContentType.Packet);
        }

        public static Hashtable SingleData(string key, object value)
        {
            Hashtable t = new Hashtable();
            t.Add(key, value);
            return t;
        }
    }

}

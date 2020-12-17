using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stx.Net.ServerOnly
{
    public interface IBaseServer<TIdentity> where TIdentity : NetworkIdentity, new()
    {
        List<string> BannedClients { get; set; }
        List<string> BannedClientIPs { get; set; }
        Dictionary<string, RequestDelegate<TIdentity>> RequestHandlers { get; }
        TaskScheduler Scheduler { get; }
        int ConnectedCount { get; }
        //ConcurrentDictionary<string, BaseClientData<TIdentity>> ConnectedClients { get; }

        void Stop();
        void Announce(string message);
        bool DisconnectClient(BaseClientData<TIdentity> client, DisconnectReason reason);
        BaseClientData<TIdentity> GetConnectedClient(string clientID);
        ICollection<BaseClientData<TIdentity>> GetConnectedClients();
        BaseClientData<TIdentity> GetConnectedClientFromName(string name);
        BaseClientData<TIdentity> GetRandomConnectedClient();
    }
}

using Stx.Net.RoomBased;
using System.Net.Sockets;

namespace Stx.Net.ServerOnly.RoomBased
{
    public class ClientData : BaseClientData<ClientIdentity>
    {
        public Server Server { get; }

        public ClientData(Server server, Socket socket) : base(socket)
        {
            Server = server;
        }

        public override void DisconnectMe(DisconnectReason reason)
        {
            if (Identity.IsInAnyRoom)
            {
                Server.Matchmaking.KickFromCurrent(Identity);
            }

            base.DisconnectMe(reason);
        }
    }
}

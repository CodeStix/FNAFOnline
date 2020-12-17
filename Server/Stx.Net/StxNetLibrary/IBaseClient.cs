using System.Net;

namespace Stx.Net
{
    public interface IBaseClient : IServerConnected
    {
        IPEndPoint Remote { get; }
        DataReceiver DataReceiver { get; set; }
        string ClientAuthorisationToken { get; }
        bool Connected { get; }
        string ApplicationName { get; }
        string ApplicationVersion { get; }

        void StopConnection();
        void LocalAnnounce(string message);
    }

    public interface IBaseClient<TIdentity> : IBaseClient where TIdentity : NetworkIdentity, new()
    {
        TIdentity You { get; }
    }
}

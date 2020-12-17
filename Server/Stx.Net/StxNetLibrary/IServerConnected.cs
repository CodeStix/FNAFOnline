namespace Stx.Net
{
    public interface IServerConnected : INetworkedItem
    {
        void SendToServer(byte[] rawBytes, BytesContentType contentType);
        void SendToServer(Packet packet);
    }
}

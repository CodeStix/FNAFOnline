namespace Stx.Net
{
    public interface INetworkedItem
    {
        string NetworkID { get; }

        Packet GetNewPacket();
        RequestPacket GetNewRequestPacket(string requestItemName, RequestPacket.RequestResponseDelegate requestResponse);
    }
}

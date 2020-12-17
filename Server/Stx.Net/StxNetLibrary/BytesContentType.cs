namespace Stx.Net
{
    public enum BytesContentType : ushort
    {
        Nothing,
        Packet,
        /// <summary>
        /// Contains information about the client that is willing to connect to the server.
        /// Only send by the client to the server.
        /// </summary>
        AuthorizationScheme,
        Ping,
        /// <summary>
        /// Contains the reason why the client will get disconnected in the first byte.
        /// Only send by the server to the client.
        /// </summary>
        DisconnectReason,
        /// <summary>
        /// Contains the location of the updated application in ASCII format.
        /// Only send by the server to the client.
        /// </summary>
        UpdateLocation, 
        Announcement
    }
}

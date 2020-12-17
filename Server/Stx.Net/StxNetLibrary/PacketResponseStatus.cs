namespace Stx.Net
{
    public enum PacketResponseStatus
    {
        Responded,
        Okey,
        Failed,
        UnknownRequest,
        RequiresCompletion
    }

    public static class PacketResponseStatusExtensions
    {
        public static bool WasSuccessful(this PacketResponseStatus status)
        {
            return status <= PacketResponseStatus.Okey;
        }

        public static bool WasUnSuccessful(this PacketResponseStatus status)
        {
            return status > PacketResponseStatus.Okey;
        }
    }
}

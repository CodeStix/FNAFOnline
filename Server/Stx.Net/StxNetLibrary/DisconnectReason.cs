namespace Stx.Net
{
    public enum DisconnectReason : byte
    {
        Unknown,
        WentOfflineIntended,
        RequestedOfflineIntended,
        TimedOut,
        HostShutdown,
        KickedByHost,
        Unauthorized,
        UpdateRequired,
        Overloaded,
        FaultyIntegrity,
        FalseIdentity,
        ExceptionThrown,
        Other
    }
}

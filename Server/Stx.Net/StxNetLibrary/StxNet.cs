using Stx.Logging;
using Stx.Serialization;
using System;

namespace Stx.Net
{
    public static class StxNet
    {
        public static bool IsClient { get; internal set; } = false;
        public static bool IsServer { get; set; } = false;

        public static ILogger DefaultLogger { get; set; } = new VoidLogger();

        public const string ClientStxVersion = "0.1.0";

        public const string DefaultApplicationName = "StxApplication";
        public const string DefaultApplicationVersion = "1.0";
        /// <summary>
        /// The default size of the receive and send buffers per client.
        /// The default is 64 kilobytes per client.
        /// </summary>
        public const int DefaultReceiveBufferSize = 65536;

        public static string ImgurApplicationClientId { get; set; } = null;
        public static string ImgurApplicationClientSecret { get; set; } = null;

        /// <summary>
        /// This method registers all the default types that will be send over network, to include your own, override this method.
        /// See <see cref="Bytifier.Include{T}"/>. Make sure you include all the types in the same order at the other side!
        /// </summary>
        public static void RegisterNetworkTypes()
        {
            Bytifier.Include<Packet>();
            Bytifier.Include<RequestPacket>();
            Bytifier.Include<RequestPacket.Completion>();
            Bytifier.Include<NetworkIdentity>();
            Bytifier.Include<BHashtable>();
            Bytifier.Include<BList<string>>();
            Bytifier.Include<BConcurrentList<string>>();
            Bytifier.IncludeEnum<PacketResponseStatus>();
        }
    }
}

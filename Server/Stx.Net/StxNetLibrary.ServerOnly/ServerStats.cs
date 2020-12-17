using Stx.Net;
using Stx.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stx.Net.ServerOnly
{
    public static class ServerStats
    {
        public static uint packets;

        public static uint requests;
        public static uint okeyRequests;
        public static uint failedRequests;
        public static uint unknownRequests;
        public static long timeSpendOnRequests; // in ms

        public static ulong pings;

        public static uint connectionsAccepted;
        public static uint registeredClients;

        public static uint disconnects;
        public static uint intendedDisconnects;
        public static uint unauthorizedDisconnects;
        public static uint falseIdentityDisconnects;
        public static uint faultyIntegrityDisconnects;
        public static uint timedOutDisconnects;
        public static uint overloadDisconnects;

        public static long bytesReceived;
        public static long bytesSend;

        public static void IncreaseFor(DisconnectReason reason)
        {
            disconnects++;
            if (reason == DisconnectReason.WentOfflineIntended || reason == DisconnectReason.RequestedOfflineIntended)
                intendedDisconnects++;
            else if (reason == DisconnectReason.Unauthorized)
                unauthorizedDisconnects++;
            else if (reason == DisconnectReason.FalseIdentity)
                falseIdentityDisconnects++;
            else if (reason == DisconnectReason.FaultyIntegrity)
                faultyIntegrityDisconnects++;
            else if (reason == DisconnectReason.TimedOut)
                timedOutDisconnects++;
            else if (reason == DisconnectReason.Overloaded)
                overloadDisconnects++;
        }

        public static string GetStringRepresentation()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Total pings send->received: " + pings);
            sb.AppendLine("Total bytes received: " + StringUtil.FormatBytes(bytesReceived));
            sb.AppendLine("Total bytes send: " + StringUtil.FormatBytes(bytesSend));
            sb.AppendLine();
            sb.AppendLine("Total packet objects received: " + packets);
            sb.AppendLine("Total requests received: " + requests);
            sb.AppendLine("Total okey requests: " + okeyRequests);
            sb.AppendLine("Total failed requests: " + failedRequests);
            sb.AppendLine("Total unknown received: " + unknownRequests);
            sb.AppendLine($"Total time spend for handling requests: { timeSpendOnRequests } ms");
            sb.AppendLine();
            sb.AppendLine("Total connections accepted: " + connectionsAccepted);
            sb.AppendLine("Total new clients registered: " + registeredClients);
            sb.AppendLine();
            sb.AppendLine("Total disconnects: " + disconnects);
            sb.AppendLine("Total intended disconnects: " + intendedDisconnects);
            sb.AppendLine("Total unauthorized disconnects: " + unauthorizedDisconnects);
            sb.AppendLine("Total false identity disconnects: " + falseIdentityDisconnects);
            sb.AppendLine("Total faulty integrity disconnects: " + faultyIntegrityDisconnects);
            sb.AppendLine("Total timed out disconnects: " + timedOutDisconnects);
            sb.AppendLine("Total overload caused disconnects: " + overloadDisconnects);

            return sb.ToString();
        }
    }
}

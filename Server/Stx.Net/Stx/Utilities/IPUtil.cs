using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Stx.Utilities
{
    public static class IPUtil
    {
        public static IPAddress GetLocalIP(AddressFamily family)
        {
            IPAddress[] addrs = Dns.GetHostAddresses(Dns.GetHostName());
            return addrs.First((e) => e.AddressFamily == family);
        }

        public static bool ParseIPAndPort(string str, out IPAddress address, out ushort port, ushort defaultPort = 80)
        {
            bool succes = true;
            if (str.Contains(":"))
            {
                string[] split = str.Split(':');

                if (!IPAddress.TryParse(split[0], out address))
                    succes = false;
                if (!ushort.TryParse(split[1], out port))
                    succes = false;
            }
            else
            {
                port = defaultPort;

                if (!IPAddress.TryParse(str, out address))
                    succes = false;
            }
            return succes;
        } 

        public static IPAddress HostToIP(string hostOrIP)
        {
            return Dns.GetHostEntry(hostOrIP).AddressList[0];
        }

        public static bool TryHostToIP(string hostOrIP, out IPAddress address)
        {
            try
            {
                address = Dns.GetHostEntry(hostOrIP).AddressList.First(addr => addr.AddressFamily == AddressFamily.InterNetwork);
                return true;
            }
            catch
            {
                address = null;
                return false;
            }
        }
    }
}

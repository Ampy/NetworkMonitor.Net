using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NetworkMonitor.Common
{
    public class SocketPacket
    {
        public string Version { get; set; }
        public int HeaderLength { get; set; }
        public int MessageLength { get; set; }
        public int TotalLength { get; set; }
        public Protocol ProtocolType { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public IPAddress SourceAddress { get; set; }
        public IPAddress DestinationAddress { get; set; }
        public string SourcePort { get; set; }
        public string DestinationPort { get; set; }
    }
}

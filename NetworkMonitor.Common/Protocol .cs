using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMonitor.Common
{
    public enum Protocol
    {
        ICMP = 1,
        IGMP = 2,
        TCP = 6,
        UDP = 17,
        Unknown = -1
    };  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NetworkMonitor.Common
{
    public class IPNotifyItem
    {
        public PackageDirect m_direct;
        public IPAddress SourceIP;
        public IPAddress DestIP;
        public Protocol m_protocol;
        public long m_len;

        public string FLAG
        {
            get
            {
                return string.Format("{0}_{1}_(2)", m_direct, SourceIP, DestIP);
            }
        }
    }
}

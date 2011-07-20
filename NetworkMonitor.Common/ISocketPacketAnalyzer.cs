using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMonitor.Common
{
    public interface ISocketPacketAnalyzer
    {
        SocketPacket Anlalyz(byte[] raw, int startPos, int length, SocketDataEventArgs e);
    }
}

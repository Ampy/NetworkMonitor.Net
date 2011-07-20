using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMonitor.Common
{
    public abstract class BaseSocketPacketAnalyzer : ISocketPacketAnalyzer
    {
        protected abstract SocketPacket DoAnalyz(byte[] raw, int startPos, int length);

        #region ISocketPacketAnalyzer Members

        public virtual SocketPacket Anlalyz(byte[] raw, int startPos, int length, SocketDataEventArgs e)
        {
            SocketPacket retValue = DoAnalyz(raw, startPos, length);

            ////把buf中的IP头赋给PacketArrivedEventArgs中的IPHeaderBuffer
            //e.MessageHeadBuff = new byte[retValue.HeaderLength];
            //Array.Copy(raw, 0, e.MessageHeadBuff, 0, retValue.HeaderLength);
            ////把buf中的包中内容赋给PacketArrivedEventArgs中的MessageBuffer
            //e.MessageBuff = new byte[retValue.MessageLength];
            //Array.Copy(raw, retValue.HeaderLength, e.MessageBuff, 0, (int)retValue.MessageLength);

            return retValue;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMonitor.Common
{
    public class SocketDataEventArgs : EventArgs
    {
        private byte[] m_MessageHeadBuff;
        private byte[] m_MessageBuff;
        private byte[] m_ReceiveBuff;

        public SocketDataEventArgs()
            : base()
        { }

        public byte[] ReveiveBuff
        {
            get { return m_ReceiveBuff; }
            set { m_ReceiveBuff = value; }
        }

        public byte[] MessageBuff
        {
            get { return m_MessageBuff; }
            set { m_MessageBuff = value; }
        }

        public byte[] MessageHeadBuff
        {
            get { return m_MessageHeadBuff; }
            set { m_MessageHeadBuff = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace NetworkMonitor.Common
{
    public class SocketSession
    {
        private Socket m_socket;
        private byte[] m_buffer;
        private int m_datalen;

        public SocketSession(Socket s, int bufflen)
        {
            m_socket = s;
            m_buffer = new byte[bufflen];
        }

        public Socket Socket
        {
            get { return m_socket; }
        }

        public byte[] Buffer
        {
            get { return m_buffer; }
            set { m_buffer = value; }
        }

        public int DataLength
        {
            get { return m_datalen; }
            set { m_datalen = value; }
        }
    }  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetworkMonitor.Common
{
    public class RawSocket : IDisposable
    {
        private Socket m_rawsock;
        const int BUFFER_SIZE = 1024 * 4;
        AsyncCallback m_ReceivedData;
        SocketProvider m_Provider;
        bool m_disposed;

        public RawSocket(AddressFamily family, SocketProvider provider)
        {
            m_rawsock = new Socket(family, SocketType.Raw, ProtocolType.IP);
            m_rawsock.Blocking = false;
            m_Provider = provider;
            m_ReceivedData = new AsyncCallback(OnReceive);
        }

        ~RawSocket()
        {
            Dispose();
        }

        public void Start(string ip)
        {
            m_rawsock.Bind(new IPEndPoint(IPAddress.Parse(ip), 0));
            SetSocketOpt();

            SocketSession ss = new SocketSession(m_rawsock, BUFFER_SIZE);
            m_rawsock.BeginReceive(ss.Buffer, 0, BUFFER_SIZE, SocketFlags.None, m_ReceivedData, ss);
        }

        private void SetSocketOpt()
        {
            m_rawsock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
            byte[] byIN = new byte[4] { 1, 0, 0, 0 };
            byte[] byOut = new byte[4];// { 1, 0, 0, 0 }; //Capture outgoing packets  
            m_rawsock.IOControl(IOControlCode.ReceiveAll, byIN, byOut);
        }

        private void OnReceive(IAsyncResult ar)
        {
            SocketSession ss = (SocketSession)ar.AsyncState;
            ss.DataLength = ss.Socket.EndReceive(ar);
            if (ss.DataLength == 0)
            {
                m_Provider.OnDropConnection();
                ss.Socket.Shutdown(SocketShutdown.Both);
                ss.Socket.Close();
                return;
            }
            m_Provider.OnReceiveData(ss);
            if (!m_disposed)
            {
                SocketSession ssnew = new SocketSession(m_rawsock, BUFFER_SIZE);
                m_rawsock.BeginReceive(ssnew.Buffer, 0, BUFFER_SIZE, SocketFlags.None, m_ReceivedData, ssnew);
            }
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                m_Provider.OnDropConnection();
                m_rawsock.Shutdown(SocketShutdown.Both);
                m_rawsock.Close();
            }
        }
    }
}

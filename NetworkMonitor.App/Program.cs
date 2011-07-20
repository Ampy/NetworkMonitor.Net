using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkMonitor.Common;
using System.Net.Sockets;

namespace NetworkMonitor.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("输入要监控的IP地址:");

                string bindingip = Console.ReadLine();

                NetProvider m_provider;
                RawSocket m_socket;

                m_provider = new NetProvider(bindingip);
                m_socket = new RawSocket(AddressFamily.InterNetwork, m_provider);
                m_socket.Start(bindingip);
            }
            catch (Exception eX)
            {
                Console.WriteLine(eX.Message);
            }

            Console.ReadLine();
        }
    }
}

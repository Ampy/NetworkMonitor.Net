using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkMonitor.Common;

namespace NetworkMonitor.App
{
    public class NetProvider : SocketProvider
    {
        public NetProvider(string bindingip)
            : base(bindingip)
        { }

        protected override void OnIPNotify(IPNotifyItem stat)
        {
            if (PackageDirect.IN == stat.m_direct)
            {
                Console.WriteLine(
                    "接受数据\t源地址:{0}\t\t目的地址:{1}\t\t包大小:{2}",
                    stat.SourceIP, stat.DestIP, stat.m_len);
            }
            else
            {
                Console.WriteLine(
                    "发送数据\t源地址:{0}\t\t目的地址:{1}\t\t包大小:{2}",
                    stat.SourceIP, stat.DestIP, stat.m_len);
            }
        }
    }
}

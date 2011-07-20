using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Net;

namespace NetworkMonitor.Common
{
    delegate void AnalyzeProc(List<SocketPacket> packages, int ticklen);
    public delegate void SpeedNotify(NetSpeed speed);
    public delegate void IPNotify(IPNotifyItem stat);

    public class AnalyzeMgr
    {
        private Timer m_scanner;
        private IPAddress m_localbindaddress;
        private List<SocketPacket> m_datalist;
        private int m_lasttick;
        private AnalyzeProc m_analyze_processor;
        private SpeedNotify m_speednotify;
        private IPNotify m_ipnotify;

        public AnalyzeMgr(string bindip, SpeedNotify speednotify, IPNotify ipnotify)
        {
            m_scanner = new Timer(1000);
            m_scanner.Enabled = false;
            m_scanner.Elapsed += new ElapsedEventHandler(m_scanner_Elapsed);

            m_analyze_processor = DoAnalyze;
            m_speednotify = speednotify;
            m_ipnotify = ipnotify;
            m_datalist = new List<SocketPacket>();

            m_localbindaddress = IPAddress.Parse(bindip);
        }

        public void Start()
        {
            m_scanner.Start();
        }

        public void Stop()
        {
            m_scanner.Stop();
        }

        public void PushPackage(SocketPacket package)
        {
            if (IPAddress.Equals(package.SourceAddress, m_localbindaddress)
               || IPAddress.Equals(package.DestinationAddress, m_localbindaddress)
              )//only process the package of local address  
            {
                lock (m_datalist)
                {
                    m_datalist.Add(package);
                }
            }
        }

        void m_scanner_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<SocketPacket> procdatalist = new List<SocketPacket>();

            lock (m_datalist)
            {
                procdatalist.AddRange(m_datalist.ToArray());
                m_datalist.Clear();
            }

            int tick = Environment.TickCount;
            if (m_lasttick == 0)
            {
                m_lasttick = tick;
                return;
            }

            int ticklen = tick - m_lasttick;
            m_lasttick = tick;
            if (procdatalist.Count > 0)
            {//异步调用  
                m_analyze_processor.BeginInvoke(procdatalist, ticklen, DoAsynAnalyze, m_analyze_processor);
            }
        }

        void DoAsynAnalyze(IAsyncResult ar)
        {
            AnalyzeProc invoker = (AnalyzeProc)ar.AsyncState;
            invoker.EndInvoke(ar);
        }

        void DoAnalyze(List<SocketPacket> packages, int ticklen)
        {
            NetSpeed speed = new NetSpeed();

            foreach (SocketPacket p in packages)
            {
                if (IPAddress.Equals(p.SourceAddress, m_localbindaddress))
                {//Send  
                    speed.nUpTotalLen += p.TotalLength;
                    speed.nUppackageCnt += 1;
                    switch (p.ProtocolType)
                    {
                        case Protocol.TCP:
                            speed.nUpTCPTotalLen += p.TotalLength;
                            speed.nUpTCPpackageCnt += 1;
                            break;
                        case Protocol.UDP:
                            speed.nUpUDPTotalLen += p.TotalLength;
                            speed.nUpUDPpackageCnt += 1;
                            break;
                        case Protocol.ICMP:
                            speed.nUpICMPTotalLen += p.TotalLength;
                            speed.nUpICMPpackageCnt += 1;
                            break;
                        case Protocol.IGMP:
                            speed.nUpIGMPTotalLen += p.TotalLength;
                            speed.nUpIGMPpackageCnt += 1;
                            break;
                    }

                    IPNotifyItem stat = new IPNotifyItem();
                    stat.m_direct = PackageDirect.OUT;
                    stat.SourceIP = p.SourceAddress;
                    stat.DestIP = p.DestinationAddress;
                    stat.m_len = p.TotalLength;
                    stat.m_protocol = p.ProtocolType;
                    if (m_ipnotify != null)
                        m_ipnotify.Invoke(stat);
                }
                else
                {//Receive  
                    speed.nDownTotalLen += p.TotalLength;
                    speed.nDownpackageCnt += 1;
                    switch (p.ProtocolType)
                    {
                        case Protocol.TCP:
                            speed.nDownTCPTotalLen += p.TotalLength;
                            speed.nDownTCPpackageCnt += 1;
                            break;
                        case Protocol.UDP:
                            speed.nDownUDPTotalLen += p.TotalLength;
                            speed.nDownUDPpackageCnt += 1;
                            break;
                        case Protocol.ICMP:
                            speed.nDownICMPTotalLen += p.TotalLength;
                            speed.nDownICMPpackageCnt += 1;
                            break;
                        case Protocol.IGMP:
                            speed.nDownIGMPTotalLen += p.TotalLength;
                            speed.nDownIGMPpackageCnt += 1;
                            break;
                    }

                    IPNotifyItem stat = new IPNotifyItem();
                    stat.m_direct = PackageDirect.IN;
                    stat.SourceIP = p.SourceAddress;
                    stat.DestIP = p.DestinationAddress;
                    stat.m_len = p.TotalLength;
                    stat.m_protocol = p.ProtocolType;
                    if (m_ipnotify != null)
                        m_ipnotify.Invoke(stat);
                }
            }

            speed.CaculateSpeed(ticklen);
            if (m_speednotify != null)
                m_speednotify.Invoke(speed);
        }
    }  
}

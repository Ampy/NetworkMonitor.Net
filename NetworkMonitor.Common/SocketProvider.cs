using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMonitor.Common
{
    public abstract class SocketProvider
    {
        private string m_BidingIP;
        private AnalyzeMgr m_AnalyzMgr;
        protected ISocketPacketAnalyzer m_SocketPacketAnalyzer;

        protected OnAnalyzeHandler BeforeAnalyz;

        protected OnAnalyzeHandler AfterAnalyz;

        public SocketProvider(string bindingip)
            : this(bindingip, new OriSocketPacketAnalyzer())
        { }

        public SocketProvider(string bindingip, ISocketPacketAnalyzer analyzer)
        {
            m_BidingIP = bindingip;
            m_AnalyzMgr = new AnalyzeMgr(bindingip, OnSpeedNotify, OnIPNotify);
            m_SocketPacketAnalyzer = analyzer;

            BeforeAnalyz = OnBeforeAnalyz;
            AfterAnalyz = OnAfterAnalyz;

            m_AnalyzMgr.Start();
        }

        protected virtual void OnBeforeAnalyz(SocketDataEventArgs e)
        { }

        protected virtual void OnAfterAnalyz(SocketDataEventArgs e)
        { }

        public virtual void OnReceiveData(SocketSession state)
        {
            SocketDataEventArgs e = new SocketDataEventArgs();
            e.ReveiveBuff = state.Buffer;

            OnBeforeAnalyz(e);

            SocketPacket sp = m_SocketPacketAnalyzer.Anlalyz(state.Buffer, 0, state.DataLength, e);

            m_AnalyzMgr.PushPackage(sp);

            OnAfterAnalyz(e);
        }

        public virtual void OnDropConnection()
        { }

        protected virtual void OnIPNotify(IPNotifyItem stat)
        { }

        protected virtual void OnSpeedNotify(NetSpeed speed)
        { }
    }
}

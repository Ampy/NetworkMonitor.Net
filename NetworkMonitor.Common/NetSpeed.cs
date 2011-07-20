using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkMonitor.Common
{
    public class NetSpeed
    {
        public long nUpTotalLen;
        public long nDownTotalLen;
        public long nUpTCPTotalLen;
        public long nDownTCPTotalLen;
        public long nUpUDPTotalLen;
        public long nDownUDPTotalLen;
        public long nUpICMPTotalLen;
        public long nDownICMPTotalLen;
        public long nUpIGMPTotalLen;
        public long nDownIGMPTotalLen;

        public int nUppackageCnt;
        public int nDownpackageCnt;
        public int nUpTCPpackageCnt;
        public int nDownTCPpackageCnt;
        public int nUpUDPpackageCnt;
        public int nDownUDPpackageCnt;
        public int nUpICMPpackageCnt;
        public int nDownICMPpackageCnt;
        public int nUpIGMPpackageCnt;
        public int nDownIGMPpackageCnt;

        //Speed: Kbps  
        public double nUpSpeed;
        public double nDownSpeed;
        public double nUpTCPSpeed;
        public double nDownTCPSpeed;
        public double nUpUDPSpeed;
        public double nDownUDPSpeed;

        public void CaculateSpeed(int ticklen)
        {
            nUpSpeed = (double)nUpTotalLen * 1000 / ticklen / 1024.0;
            nDownSpeed = (double)nDownTotalLen * 1000 / ticklen / 1024.0;
            nUpTCPSpeed = (double)nUpTCPTotalLen * 1000 / ticklen / 1024.0;
            nDownTCPSpeed = (double)nDownTCPTotalLen * 1000 / ticklen / 1024.0;
            nUpUDPSpeed = (double)nUpUDPTotalLen * 1000 / ticklen / 1024.0;
            nDownUDPSpeed = (double)nDownUDPTotalLen * 1000 / ticklen / 1024.0;
        }
    }
}

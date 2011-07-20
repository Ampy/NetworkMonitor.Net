using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NetworkMonitor.Common
{
    public class OriSocketPacketAnalyzer : BaseSocketPacketAnalyzer
    {
        private string GetIPAddress(byte[] bArray, int nStart)
        {
            byte[] tmp = new byte[4];

            if (bArray.Length > nStart + 2)
            {
                tmp[0] = bArray[nStart];
                tmp[1] = bArray[nStart + 1];
                tmp[2] = bArray[nStart + 2];
                tmp[3] = bArray[nStart + 3];
            }

            return tmp[0] + "." + tmp[1] + "." + tmp[2] + "." + tmp[3];
        }

        protected override SocketPacket DoAnalyz(byte[] raw, int startPos, int length)
        {
            byte temp_protocol = 0;
            uint temp_version = 0;
            uint temp_ip_srcaddr = 0;
            uint temp_ip_destaddr = 0;
            short temp_srcport = 0;
            short temp_dstport = 0;
            IPAddress temp_ip;

            SocketPacket retValue = new SocketPacket();

            unsafe
            {
                fixed (byte* fixed_buf = raw)
                {
                    IP4Header_V2* head = (IP4Header_V2*)fixed_buf;//把数据流整和为IPHeader结构
                    retValue.HeaderLength = (head->ip_verlen & 0x0F) << 2;

                    temp_protocol = head->ip_protocol;
                    switch (temp_protocol)//提取协议类型
                    {
                        case 1: retValue.ProtocolType = Protocol.ICMP; break;
                        case 2: retValue.ProtocolType = Protocol.IGMP; break;
                        case 6: retValue.ProtocolType = Protocol.TCP; break;
                        case 17: retValue.ProtocolType = Protocol.UDP; break;
                        default: retValue.ProtocolType = Protocol.Unknown; break;
                    }

                    temp_version = (uint)(head->ip_verlen & 0xF0) >> 4;//提取IP协议版本
                    retValue.Version = temp_version.ToString();

                    //以下语句提取出了PacketArrivedEventArgs对象中的其他参数
                    temp_ip_srcaddr = head->ip_srcaddr;
                    temp_ip_destaddr = head->ip_destaddr;

                    temp_ip = new IPAddress(temp_ip_srcaddr);
                    retValue.Source = temp_ip.ToString();
                    retValue.SourceAddress = temp_ip;

                    temp_ip = new IPAddress(temp_ip_destaddr);
                    retValue.Destination = temp_ip.ToString();
                    retValue.DestinationAddress = temp_ip;

                    temp_srcport = *(short*)&fixed_buf[retValue.HeaderLength];
                    temp_dstport = *(short*)&fixed_buf[retValue.HeaderLength + 2];
                    retValue.SourcePort = IPAddress.NetworkToHostOrder(temp_srcport).ToString();
                    retValue.DestinationPort = IPAddress.NetworkToHostOrder(temp_dstport).ToString();

                    retValue.TotalLength = length;
                    retValue.MessageLength = length - retValue.HeaderLength;
                }
            }

            return retValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;

namespace NetworkMonitor.Common
{
    public class SimpleSocketPacketAnalyzer : BaseSocketPacketAnalyzer
    {
        const int Size_IPHeader = 18;

        private IP4Header m_header;
        private byte[] m_data;
        private int m_nHeadLength;
        private int m_packagelength;

        private static IP4Header GetIPHeader(byte[] buffer, int startpos, int datalen)
        {
            if (datalen < Size_IPHeader)
                throw new ArgumentException("data length error");
            byte[] tmpdata = new byte[datalen];
            Buffer.BlockCopy(buffer, startpos, tmpdata, 0, datalen);
            return ByteBufferToStruct<IP4Header>(tmpdata);
        }

        public static unsafe byte[] StructToByteBuffer(object o)
        {
            int size = Marshal.SizeOf(o);
            IntPtr ip = Marshal.AllocHGlobal(size); //WARNING: ALLOCATING UNMANAGED MEMORY!  
            Marshal.StructureToPtr(o, ip, false);
            byte[] buffer = new byte[size];
            Marshal.Copy(ip, buffer, 0, size);
            Marshal.FreeHGlobal(ip); //UNALLOCATE - DO NOT REMOVE THIS!!!!!  
            return buffer;
        }

        public static unsafe T ByteBufferToStruct<T>(byte[] data)
        {
            IntPtr ip = Marshal.AllocHGlobal(data.Length);  //WARNING: ALLOCATING UNMANAGED MEMORY!  
            Marshal.Copy(data, 0, ip, data.Length);

            T o = (T)Marshal.PtrToStructure(ip, typeof(T));
            Marshal.FreeHGlobal(ip);  //UNALLOCATE - DO NOT REMOVE THIS!!!!!  
            return o;
        }

        public static ushort NetworkToHostOrder(ushort val)
        {
            byte[] array = BitConverter.GetBytes(val);
            Array.Reverse(array);
            return BitConverter.ToUInt16(array, 0);
        }

        protected override SocketPacket DoAnalyz(byte[] raw, int startPos, int length)
        {
            m_packagelength = length;

            m_header = GetIPHeader(raw, startPos, length);
            m_header.ip_totallength = NetworkToHostOrder(m_header.ip_totallength);
            m_header.ip_id = NetworkToHostOrder(m_header.ip_id);
            m_header.ip_offset = NetworkToHostOrder(m_header.ip_offset);
            m_header.ip_checksum = NetworkToHostOrder(m_header.ip_checksum);

            m_nHeadLength = m_header.ip_verlen;
            m_nHeadLength &= 0x0f;
            m_nHeadLength *= 4;//Multiply by four to get the exact header length  
            m_data = new byte[m_header.ip_totallength - m_nHeadLength];
            Buffer.BlockCopy(raw, startPos + m_nHeadLength, m_data, 0, m_data.Length);

            SocketPacket retValue = new SocketPacket();

            retValue.Destination = "";
            retValue.DestinationAddress = (new IPAddress(m_header.ip_destaddr));
            retValue.DestinationPort = "";
            retValue.HeaderLength = m_nHeadLength;
            retValue.MessageLength = m_header.ip_totallength - m_nHeadLength;
            retValue.ProtocolType = m_header.ip_protocol == 1 || m_header.ip_protocol == 2
                || m_header.ip_protocol == 6 || m_header.ip_protocol == 17 ? (Protocol)m_header.ip_protocol : Protocol.Unknown;
            retValue.Source = "";
            retValue.SourceAddress = (new IPAddress(m_header.ip_srcaddr));
            retValue.SourcePort = "";
            retValue.TotalLength = m_header.ip_totallength;
            retValue.Version = (m_header.ip_verlen >> 4) == 4 ? "IP v4" :
                (m_header.ip_verlen >> 4) == 6 ? "IP v6" : "Unknown";

            return retValue;
        }
    }
}

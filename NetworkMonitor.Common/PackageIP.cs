using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.InteropServices;

namespace NetworkMonitor.Common
{
    public class PackageIP
    {
        private IP4Header m_header;
        private byte[] m_data;
        private int m_nHeadLength;
        private int m_packagelength;

        public PackageIP(byte[] buffer, int startpos, int datalen)
        {
            m_packagelength = datalen;

            m_header = GetIPHeader(buffer, startpos, datalen);
            m_header.ip_totallength = NetworkToHostOrder(m_header.ip_totallength);
            m_header.ip_id = NetworkToHostOrder(m_header.ip_id);
            m_header.ip_offset = NetworkToHostOrder(m_header.ip_offset);
            m_header.ip_checksum = NetworkToHostOrder(m_header.ip_checksum);

            m_nHeadLength = m_header.ip_verlen;
            m_nHeadLength &= 0x0f;
            m_nHeadLength *= 4;//Multiply by four to get the exact header length  
            m_data = new byte[m_header.ip_totallength - m_nHeadLength];
            Buffer.BlockCopy(buffer, startpos + m_nHeadLength, m_data, 0, m_data.Length);
        }

        public string Version
        {
            get
            {
                if ((m_header.ip_verlen >> 4) == 4)
                {
                    return "IP v4";
                }
                else if ((m_header.ip_verlen >> 4) == 6)
                {
                    return "IP v6";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string HeaderLength
        {
            get
            {
                return m_nHeadLength.ToString();
            }
        }

        public ushort MessageLength
        {
            get
            {
                return (ushort)(m_header.ip_totallength - m_nHeadLength);
            }
        }

        public string DifferentiatedServices
        {
            get
            {
                return string.Format("0x{0:x2} ({1})", m_header.ip_tos, m_header.ip_tos);
            }
        }

        public string Flags
        {
            get
            {
                int nFlags = m_header.ip_offset >> 13;
                if (nFlags == 2)
                {
                    return "Don't fragment";
                }
                else if (nFlags == 1)
                {
                    return "More fragments to come";
                }
                else
                {
                    return nFlags.ToString();
                }
            }
        }

        public string FragmentationOffset
        {
            get
            {
                int nOffset = m_header.ip_offset & 0x07ff;
                return nOffset.ToString();
            }
        }

        public string TTL
        {
            get
            {
                return m_header.ip_ttl.ToString();
            }
        }

        public Protocol ProtocolType
        {
            get
            {
                switch (m_header.ip_protocol)
                {
                    case 1:
                    case 2:
                    case 6:
                    case 17:
                        return (Protocol)m_header.ip_protocol;
                    default:
                        return Protocol.Unknown;
                }
            }
        }

        public string Checksum
        {
            get
            {
                return string.Format("0x{0:x2}", m_header.ip_checksum);
            }
        }

        public IPAddress SourceAddress
        {
            get
            {
                return new IPAddress(m_header.ip_srcaddr);
            }
        }

        public IPAddress DestinationAddress
        {
            get
            {
                return new IPAddress(m_header.ip_destaddr);
            }
        }

        public long TotalLength
        {
            get
            {
                return m_header.ip_totallength;
            }
        }

        public string Identification
        {
            get
            {
                return m_header.ip_id.ToString();
            }
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            ret.AppendLine(string.Format("/t/t Version:{0}", Version));
            ret.AppendLine(string.Format("/t/t HeaderLength:{0}", HeaderLength));
            ret.AppendLine(string.Format("/t/t TotalLength:{0}", TotalLength));
            ret.AppendLine(string.Format("/t/t MessageLength:{0}", MessageLength));
            ret.AppendLine(string.Format("/t/t TOS:{0}", DifferentiatedServices));
            ret.AppendLine(string.Format("/t/t Identification:{0}", Identification));
            ret.AppendLine(string.Format("/t/t Flags:{0}", Flags));
            ret.AppendLine(string.Format("/t/t FragmentationOffset:{0}", FragmentationOffset));
            ret.AppendLine(string.Format("/t/t TTL:{0}", TTL));
            ret.AppendLine(string.Format("/t/t ProtocolType:{0}", ProtocolType));
            ret.AppendLine(string.Format("/t/t Checksum:{0}", Checksum));
            ret.AppendLine(string.Format("/t/t SourceAddress:{0}", SourceAddress));
            ret.AppendLine(string.Format("/t/t DestinationAddress:{0}", DestinationAddress));
            return ret.ToString();
        }

        const int Size_IPHeader = 18;

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
    }  
}

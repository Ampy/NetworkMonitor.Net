using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetworkMonitor.Common
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IP4Header
    {
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte ip_verlen;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte ip_tos;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort ip_totallength;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort ip_id;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort ip_offset;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte ip_ttl;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte ip_protocol;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort ip_checksum;
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint ip_srcaddr;
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint ip_destaddr;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct IP4Header_V2
    {
        [FieldOffset(0)]
        public byte ip_verlen;        //I4位首部长度+4位IP版本号
        [FieldOffset(1)]
        public byte ip_tos;            //8位服务类型TOS
        [FieldOffset(2)]
        public ushort ip_totallength; //16位数据包总长度（字节）
        [FieldOffset(4)]
        public ushort ip_id;             //16位标识
        [FieldOffset(6)]
        public ushort ip_offset;       //3位标志位
        [FieldOffset(8)]
        public byte ip_ttl;            //8位生存时间 TTL
        [FieldOffset(9)]
        public byte ip_protocol;    //8位协议(TCP, UDP, ICMP, Etc.)
        [FieldOffset(10)]
        public ushort ip_checksum; //16位IP首部校验和
        [FieldOffset(12)]
        public uint ip_srcaddr;     //32位源IP地址
        [FieldOffset(16)]
        public uint ip_destaddr;   //32位目的IP地址
    }
}

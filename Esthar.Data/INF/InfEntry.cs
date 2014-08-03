using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class InfEntry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string Name;
        public byte Direction;
        public uint Unknown;
        public uint? PvP;
        public ushort HeightCameraFocus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public RectInt16[] CameraRanges;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public RectInt16[] ScreenRanges;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public InfGateway[] Gateways;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public InfTrigger[] Triggers;
    }
}
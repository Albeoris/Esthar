using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class FieldInfo
    {
        public string Name;
        public byte Direction;
        public ushort FocusHeight;
        public RectInt16[] CameraRanges;
        public RectInt16[] ScreenRanges;
        public FieldGateway[] Gateways;
        public FieldTrigger[] Triggers;
        public uint? PvP;
        public uint Unknown;

        public FieldInfo(string name, byte direction, ushort focusHeight,
            RectInt16[] cameraRanges, RectInt16[] screenRanges, FieldGateway[] gateways, FieldTrigger[] triggers,
            uint? pvp, uint unknown)
        {
            Name = name;
            Direction = direction;
            FocusHeight = focusHeight;
            CameraRanges = cameraRanges;
            ScreenRanges = screenRanges;
            Gateways = gateways;
            Triggers = triggers;
            PvP = pvp;
            Unknown = unknown;
        }
    }
}
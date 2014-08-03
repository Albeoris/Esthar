using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class InfFileReader : GameFileReader
    {
        public InfEntry Entry;

        public InfFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Entry = null;
        }

        public override void Open()
        {
            Close();

            BinaryReader br = IOStream.GetBinaryReader();
            InfEntry entry = new InfEntry();

            byte[] buff = new byte[10];
            IOStream.EnsureRead(buff, 0, 10);

            entry.Name = Encoding.ASCII.GetString(buff, 0, 9).TrimEnd('\0');
            entry.Direction = buff[9];

            entry.Unknown = br.ReadUInt32();
            if (IOStream.Length == 676)
                entry.PvP = br.ReadUInt32();

            entry.HeightCameraFocus = br.ReadUInt16();
            if (IOStream.Length == 504)
            {
                entry.CameraRanges = IOStream.ReadStructs<RectInt16>(1);
            }
            else
            {
                entry.CameraRanges = IOStream.ReadStructs<RectInt16>(8);
                entry.ScreenRanges = IOStream.ReadStructs<RectInt16>(2);
            }

            entry.Gateways = IOStream.Length < 672 ? ReadSmallGateways() : IOStream.ReadStructs<InfGateway>(12);
            entry.Triggers = IOStream.ReadStructs<InfTrigger>(12);

            if (!IOStream.IsEndOfStream())
                throw new Exception();

            Entry = entry;
        }

        private InfGateway[] ReadSmallGateways()
        {
            BinaryReader br = IOStream.GetBinaryReader();
            InfGateway[] result = new InfGateway[12];
            for (int i = 0; i < result.Length; i++)
                result[i] = new InfGateway
                {
                    SourceLine = IOStream.ReadStruct<Line3>(),
                    TargetPoint = IOStream.ReadStruct<Vector3>(),
                    TargetFieldID = br.ReadUInt16(),
                    Unknown2 = br.ReadInt32()
                };
            return result;
        }
    }
}
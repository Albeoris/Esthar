using System;
using System.IO;
using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class CaFileReader : GameFileReader
    {
        public CaCamera[] Rects;

        public CaFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Rects = null;
        }

        public override void Open()
        {
            Close();

            if (IOStream.Length % 40 == 0)
            {
                Rects = IOStream.ReadStructsByTotalSize<CaCamera>(IOStream.Length);
            }
            else if (IOStream.Length % 38 == 0)
            {
                long count = IOStream.Length / 38;
                Rects = new CaCamera[count];

                byte[] buff = new byte[40];
                using (SafeGCHandle handle = new SafeGCHandle(buff, GCHandleType.Pinned))
                    for (int i = 0; i < count; i++)
                    {
                        IOStream.EnsureRead(buff, 0, 38);
                        Rects[i] = (CaCamera)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), TypeCache<CaCamera>.Type);
                    }
            }
            else
            {
                throw new Exception("Неизвестный тип камеры.");
            }
        }
    }
}
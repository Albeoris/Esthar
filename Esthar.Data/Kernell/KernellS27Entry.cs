﻿using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS27Entry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] Unknown;

        //01 02 04 02 07 02 15 02 01 03 04 03 07 03 33 01
        //02 02 05 02 08 02 16 01 16 03 05 03 08 03 33 01
        //03 02 06 02 09 02 17 01 03 03 06 03 09 03 34 01
        //0A 03 0B 03 0C 02 33 01 03 03 06 03 09 03 35 01
        //0A 03 0B 03 0C 03 0D 03 0E 02 0F 02 11 01 35 01
        //0C 03 0D 03 0E 02 0F 02 10 02 11 01 12 01 35 01
        //0D 03 0E 03 0F 02 10 01 11 01 12 01 13 01 35 01
        //0E 03 0F 03 10 01 11 02 12 01 13 02 35 01 12 02
        //10 02 0E 03 13 01 13 02 13 01 13 02 13 02 13 02
        //26 03 28 03 29 03 1B 03 26 03 28 03 29 03 33 01
        //1D 03 20 03 23 03 2C 03 26 03 28 03 29 03 33 01
        //2A 03 2F 03 1A 03 1D 03 20 03 23 03 2C 02 34 01
        //1C 03 1E 03 21 03 24 03 2A 03 27 03 17 03 34 01
        //2B 03 2D 03 25 03 1C 03 1E 03 21 03 24 03 35 01
        //2E 03 2B 03 2D 03 30 03 31 03 17 03 1F 03 22 03
        //0E 03 10 02 13 01 13 02 10 03 13 02 13 03 38 01        
    }
}
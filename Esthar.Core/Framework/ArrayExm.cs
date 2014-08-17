using System;
using System.Runtime.InteropServices;

namespace Esthar.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class UnsafeCastContainsOnlyValueTypesAttribute : Attribute
    {
    }

    public static class ArrayExm
    {
        public static T[] UnsafeInplaceCast<T>(this byte[] array)
        {
            unsafe
            {
                fixed (void* ptrArray = array)
                {
                    ArrayHeader* ptrHeader = (ArrayHeader*)ptrArray - 1;

                    ptrHeader->Length = (IntPtr)(array.Length / TypeCache<T>.UnsafeSize);
                    ptrHeader->TypeHandle = TypeCache<T[]>.Type.TypeHandle.Value;
                }
            }

            return (T[])(object)array;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ArrayHeader
        {
            public IntPtr TypeHandle;
            public IntPtr Length;
        }
    }
}
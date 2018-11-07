using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BarbaricCode
{
    namespace Networking {
        public static class NetworkSerializer
        {
            public static T GetStruct<T>(byte[] bytes) where T : struct
            {
                var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                try
                {
                    return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                }
                finally
                {
                    handle.Free();
                }
            }

            public static byte[] GetBytes<T>(T str) where T : struct
            {
                int size = Marshal.SizeOf(str);

                byte[] arr = new byte[size];

                GCHandle h = default(GCHandle);

                try
                {
                    h = GCHandle.Alloc(arr, GCHandleType.Pinned);

                    Marshal.StructureToPtr(str, h.AddrOfPinnedObject(), false);
                }
                finally
                {
                    if (h.IsAllocated)
                    {
                        h.Free();
                    }
                }

                return arr;
            }

            public static byte[] Combine(params byte[][] arrays)
            {
                byte[] rv = new byte[arrays.Sum(a => a.Length)];
                int offset = 0;
                foreach (byte[] array in arrays)
                {
                    System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                    offset += array.Length;
                }
                return rv;
            }
        }
    }
}

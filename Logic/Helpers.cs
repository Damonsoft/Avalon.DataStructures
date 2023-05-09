using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalon.DataStructures.Logic
{
    public static class Helpers
    {
        public static unsafe T[] ResizeArray<T>(ref T[] target, int size, bool pinned = false)
        {
            T[] larray = target;
            T[] result = GC.AllocateArray<T>(size, pinned);
            Buffer.MemoryCopy(
                Unsafe.AsPointer(ref larray[0]),
                Unsafe.AsPointer(ref result[0]),
                size,
                (uint)Math.Min(size, larray.Length));
            target = result;
            return result;
        }

    }
}

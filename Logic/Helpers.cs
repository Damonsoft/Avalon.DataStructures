﻿using System.Runtime.CompilerServices;

namespace Avalon.DataStructures.Logic
{
    public static class Helpers
    {
        public static unsafe T[] ResizeArray<T>(ref T[] target, int size, bool pinned = false) where T : unmanaged
        {
            T[] src = target;
            T[] dst = GC.AllocateArray<T>(size, pinned);
            int newSize = sizeof(T) * size;
            int oldSize = sizeof(T) * target.Length;
            Buffer.MemoryCopy(
                Unsafe.AsPointer(ref src[0]),
                Unsafe.AsPointer(ref dst[0]),
                newSize,
                (uint)Math.Min(newSize, oldSize));
            target = dst;
            return dst;
        }
    }
}

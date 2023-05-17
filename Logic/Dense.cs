using Avalon.DataStructures.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalon.DataStructures.Logic
{
    public static class Dense
    {
        public static int Get(int[] data, int offset)
        {
            int length = (data[0]);

            if (offset < length)
            {
                return data[(offset + 1) * 2];
            }
            throw new IndexOutOfRangeException();
        }

        public static unsafe int Get(int* data, int offset)
        {
            if (offset < data[-2])
            {
                return data[offset * 2];
            }
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetUnsafe(int[] data, int offset, int value)
        {
            return data[(offset + 1) * 2] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SetLength(int[] data, int value)
        {
            return data[0] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetUnchecked(long[] data, int offset) => (int)(data[offset + 1]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int GetUnchecked(int* data, int offset) => data[offset * 2];

        public static int Add(ref int[] data, int value)
        {
            //
            int slot = Dense.GetLength(data);
            int size = Dense.GetLength(data) + 1;

            //
            if (size >= Sparse.GetLength(data))
            {
                //
                Sparse.ResizeSet(ref data, (size + 1) * 2);

                //
                return Add(ref data, value);
            }
            //
            Dense.SetLength(data, size);
            //
            Dense.SetUnsafe(data, slot, value);
            //
            return slot;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLength(long[] data) => (int)data[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLength(int[] data) => data[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int GetLength(int* data) => data[-2];
    }
}

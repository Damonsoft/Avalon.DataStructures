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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get(long[] data, int offset)
        {
            int length = (int)(data[0]);

            if (offset < length)
            {
                return (int)(data[offset + 1]);
            }
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Get(int* data, int offset)
        {
            if (offset < data[-2])
            {
                return data[offset * 2];
            }
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetUnchecked(long[] data, int offset) => (int)(data[offset + 1]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int GetUnchecked(int* data, int offset) => data[offset * 2];

        public static int Add(ref long[] data, int value)
        {
            //
            DenseWrap dense = DenseWrap.Create(data);
            SparseWrap sparse = SparseWrap.Create(data);

            //
            int slot = dense.Length;
            int size = dense.Length + 1;

            //
            if (size >= sparse.Length)
            {
                //
                Sparse.ResizeSet(ref data, size * 2);

                //
                return Add(ref data, value);
            }
            //
            dense.Length = size;
            //
            dense[slot] = value;
            //
            return slot;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLength(long[] data) => (int)data[0];


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int GetLength(int* data) => data[-2];
    }
}

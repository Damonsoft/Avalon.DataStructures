using Avalon.DataStructures.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalon.DataStructures.Logic
{
    public static partial class Sparse
    {
        public static long[] Create(int capacity)
        {
            long[] data = GC.AllocateArray<long>(capacity, true);
            data[0] = (data.LongLength - 1 << 32);
            return data;
        }

        public static void Init(long[] data)
        {
            //
            data[0] = (data.LongLength - 1 << 32);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Add(ref long[] data, int offset)
        {
            //
            int dense = Dense.Add(ref data, offset);
            //
            Sparse.Set(ref data, offset, dense);
            //
            return dense;
        }


        public static void Remove(long[] data, int offset)
        {
            //
            if (Sparse.Contains(data, offset))
            {
                //
                DenseWrap d_w = DenseWrap.Create(data);
                SparseWrap s_w = SparseWrap.Create(data);

                //
                int d_o = s_w[offset];
                //
                int length = d_w.Length - d_o - 1;
                //
                for (int i = 0; i < length; i++)
                {
                    // 
                    ref int d_v = ref d_w[d_o + i + 1];
                    // 
                    s_w[d_v] = d_o + i;
                    //
                    d_w[d_o + i] = d_v;
                }
                //
                s_w[offset] = 0;
                //
                int old_length = d_w.Length;
                //
                d_w[old_length - 1] = 0;
                //
                d_w.Length = old_length - 1;
                //
                return;
            }
            //
            throw new Exception($"Sparse and dense sets don't match at offset {offset}");
        }

        public static void RemoveInPlace(long[] data, int offset)
        {
            //
            int sparse = Sparse.Get(data, offset);
            //
            int dense = Dense.Get(data, sparse);

            //
            if (sparse == dense)
            {
                //
                DenseWrap d_w = DenseWrap.Create(data);
                SparseWrap s_w = SparseWrap.Create(data);

                //
                s_w[d_w[offset]] = 0;
                d_w[offset] = 0;
                //
                return;
            }
            //
            throw new Exception($"Sparse and dense sets don't match at offset {offset}");
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get(long[] data, int offset)
        {
            return (int)(data[offset + 1] >> 32);
        }

        public static int Set(ref long[] data, int offset, int value)
        {
            //
            SparseWrap sparse = SparseWrap.Create(data);

            //
            if (offset >= sparse.Length)
            {
                //
                ResizeSet(ref data, offset * 2);

                //
                return Set(ref data, offset, value);
            }
            //
            Debug.Assert(offset + 1 < data.Length);

            //
            return sparse[offset] = value;
        }

        public static int ResizeSet(ref long[] data, int size)
        {
            //
            Helpers.ResizeArray(ref data, size, true);

            //
            return (int)(data[0] = ((long)(size - 1) << 32) + (int)data[0]);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Contains(long[] data, Entity index)
        {
            //
            int* ptr = (int*)Unsafe.AsPointer(ref data[1]);

            // As long as the index is within
            // the bounds of the array it's
            // the call should be valid.
            if (index >= ptr[-1]) return false;

            //
            int d_i = ptr[index * 2 + 1];

            //
            if (d_i >= ptr[-2]) return false;

            //
            int s_i = ptr[d_i * 2];

            //
            return s_i == index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Contains(int* ptr, Entity index)
        {
            //
            if (index >= ptr[-1]) return false;

            //
            return ptr[ptr[index * 2 + 1] * 2] == index;
        }
    }
}

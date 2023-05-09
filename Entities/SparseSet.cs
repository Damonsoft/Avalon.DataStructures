using Avalon.DataStructures.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalon.DataStructures.Entities
{
    public readonly unsafe struct SparseSet
    {
        public readonly int SparseLength => _Internal[-1];
        public readonly int DenseLength => _Internal[-2];

        // We keep the address of 
        // the start of the body
        // to allow for high performance
        // in specific scenarios.
        // *** ASSUMES THE ARRAY IS PINNED ***
        private readonly int* _Internal;

        // We stash a reference so
        // the array won't be collected
        // during the lifetime of the
        // set as it's still managed
        // by the GarbageCollector;
        private readonly long[] _Reference;

        private SparseSet(int* Internal, long[] Reference)
        {
            this._Internal = Internal;
            this._Reference = Reference;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWithinSparse(int index)
        {
            return index < _Internal[-1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsWithinDense(int index)
        {
            return index < _Internal[-2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int index)
        {
            if (index < _Internal[-1])
            {
                int dense_index = _Internal[index * 2 + 1];

                if (dense_index < _Internal[-2])
                {
                    return _Internal[dense_index * 2] == dense_index;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSparse(int index)
        {
            if (index < _Internal[-1])
            {
                return _Internal[index * 2 + 1];
            }
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDense(int index)
        {
            if (index < _Internal[-2])
            {
                return _Internal[index * 2];
            }
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSparseUnsafe(int index) => _Internal[index * 2 + 1];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseUnsafe(int index) => _Internal[index * 2];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SparseWrap WrapSparse() => new SparseWrap(&_Internal[-2]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DenseWrap WrapDense() => new DenseWrap(&_Internal[-2]);

        public static SparseSet Create(int length)
        {
            long[] set = Sparse.Create(length);

            return new SparseSet((int*)Unsafe.AsPointer(ref set[1]), set);
        }

        public static void Resize(ref SparseSet set, int length)
        {
            long[] _internal = set._Reference;

            // Resize the array and preserve
            // the pinned aspect of the next array.
            Helpers.ResizeArray(ref _internal, length, pinned: true);

            // Write the new length of the
            // array to the header of the
            // buffer while preserving the
            // length of the dense set.
            _internal[0] = ((long)(length - 1) << 32) + (int)_internal[0];

            // Overwrite the original set
            // from the caller with the data.
            set = new SparseSet((int*)Unsafe.AsPointer(ref _internal[1]), _internal); ;
        }
    }

}

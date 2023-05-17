using Avalon.DataStructures.Logic;
using System.Runtime.CompilerServices;

namespace Avalon.DataStructures.Entities
{
    public readonly unsafe struct SparseSet
    {
        public readonly int SparseLength => Body[-1];
        public readonly int DenseLength => Body[-2];

        // We keep the address of 
        // the start of the body
        // to allow for high performance
        // in specific scenarios.
        // *** ASSUMES THE ARRAY IS PINNED ***
        public readonly int* Body;

        // We stash a reference so
        // the array won't be collected
        // during the lifetime of the
        // set as it's still managed
        // by the GarbageCollector;
        internal readonly int[] _Reference;

        private SparseSet(int* Internal, int[] Reference)
        {
            this.Body = Internal;
            this._Reference = Reference;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsWithinSparse(int index)
        {
            return index < Body[-1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsWithinDense(int index)
        {
            return index < Body[-2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(int index)
        {
            if (index < Body[-1])
            {
                int dense_index = Body[index * 2 + 1];

                if (dense_index < Body[-2])
                {
                    return Body[dense_index * 2] == index;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetSparse(int index)
        {
            if (index < Body[-1])
            {
                return Body[index * 2 + 1];
            }
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetDense(int index)
        {
            if (index < Body[-2])
            {
                return Body[index * 2];
            }
            throw new IndexOutOfRangeException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetSparseUnsafe(int index) => Body[index * 2 + 1];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetDenseUnsafe(int index) => Body[index * 2];
        
        public static SparseSet Create(int length)
        {
            int[] set = Sparse.Create(length, true);

            return new SparseSet((int*)Unsafe.AsPointer(ref set[2]), set);
        }

        public static int Add(ref SparseSet set, int offset)
        {
            int[] array = set._Reference;

            int result = Sparse.Add(ref array, offset);

            if (array != set._Reference)
            {
                set = new SparseSet((int*)Unsafe.AsPointer(ref array[2]), array);
            }
            return result;
        }

        public static void Remove(SparseSet set, int offset)
        {
            Sparse.Remove(set._Reference, offset);
        }

        public static void Resize(ref SparseSet set, int length)
        {
            int[] _internal = set._Reference;

            // Resize the array and preserve
            // the pinned aspect of the next array.
            Helpers.ResizeArray(ref _internal, length, pinned: true);

            // Write the new length of the
            // array to the header of the
            // buffer while preserving the
            // length of the dense set.
            _internal[0] = Sparse.Write(_internal, -1, length - 1);

            // Overwrite the original set
            // from the caller with the data.
            set = new SparseSet((int*)Unsafe.AsPointer(ref _internal[1]), _internal); ;
        }
    }

}

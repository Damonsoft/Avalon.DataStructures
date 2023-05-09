using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalon.DataStructures.Entities
{
    public readonly unsafe ref struct  SparseWrap
    {
        public readonly ref int Length => ref _pointer[1];
        public readonly ref int this[int index] => ref _pointer[((index + 1) * 2) + 1];

        readonly int* _pointer;

        internal SparseWrap(int* pointer)
        {
            this._pointer = pointer;
        }

        public static SparseWrap Create(long[] set)
        {
            return new SparseWrap((int*)Unsafe.AsPointer(ref set[0]));
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalon.DataStructures.Entities
{
    public readonly unsafe ref struct DenseWrap
    {
        public readonly ref int Length => ref _pointer[0];
        public readonly ref int this[int index] => ref _pointer[(index + 1) * 2];

        readonly int* _pointer;

        internal DenseWrap(int* pointer)
        {
            this._pointer = pointer;
        }

        public static DenseWrap Create(long[] set)
        {
            return new DenseWrap((int*)Unsafe.AsPointer(ref set[0]));
        }
    }

}

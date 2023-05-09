# A C# Sparse Set Implementation

Enables one to write a simple integer/index based sparse set implementation.

```csharp
using Avalon.DataStructures.Entities;
using System;

public class Program
{
    public static void Main()
    {
        SparseSet set = SparseSet.Create(10);

        SparseSet.Add(ref set, 1);
        SparseSet.Add(ref set, 2);
        SparseSet.Add(ref set, 3);
        SparseSet.Add(ref set, 4);
        SparseSet.Add(ref set, 5);
        SparseSet.Add(ref set, 6);

        SparseSet.Remove(ref set, 2);
        SparseSet.Remove(ref set, 4);
        SparseSet.Remove(ref set, 6);

        for (int i = 0; i < 10; i++)
        {
            if (set.Contains(i))
            {
                Console.WriteLine($"The set contains {i}.");
            }
            else
            {
                Console.WriteLine($"The set does not contain {i}.");
            }
        }
    }
}
```
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avtoobves.Infrastructure
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            var elements = source.ToArray();
            // Note i > 0 to avoid final pointless iteration
            for (var i = elements.Length - 1; i > 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                var swapIndex = rng.Next(i + 1);
                (elements[i], elements[swapIndex]) = (elements[swapIndex], elements[i]);
            }
            
            // Lazily yield (avoiding aliasing issues etc)
            foreach (var element in elements)
            {
                yield return element;
            }
        }
    }
}
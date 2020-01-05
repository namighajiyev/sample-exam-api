using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleExamIntegrationTests.Helpers
{
    public static class EnumerableExtentions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            var rnd = Utils.NewRandom();
            return enumerable.OrderBy(x => rnd.Next());
        }
    }
}
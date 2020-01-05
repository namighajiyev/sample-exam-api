using System;

namespace SampleExamIntegrationTests.Helpers
{
    public static class Utils
    {
        public static Random NewRandom()
        {
            return new Random(Guid.NewGuid().GetHashCode());
        }
    }
}
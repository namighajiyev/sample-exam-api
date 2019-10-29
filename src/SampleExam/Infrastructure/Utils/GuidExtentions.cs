using System;

namespace SampleExam.Infrastructure.Utils
{
    public static class GuidExtentions
    {
        public static string ToGuidString(this Guid guid)
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}
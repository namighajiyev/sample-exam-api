using System;
using System.Linq;
using SampleExam.Domain;
using SampleExam.Infrastructure.Data;
using SampleExam.Infrastructure.Utils;

namespace SampleExamIntegrationTests.Helpers
{
    public static class SampleExamContextHelper
    {
        public static Tag[] SeededTags { get; private set; }

        public static void SeedContext(SampleExamContext context)
        {
            var uniqueString = Guid.NewGuid().ToGuidString();
            var tags = new Tag[] {
                new Tag() { TagId = $"{uniqueString}_tag1", CreatedAt = DateTime.UtcNow },
                new Tag() { TagId = $"{uniqueString}_tag2", CreatedAt = DateTime.UtcNow },
                new Tag() { TagId = $"{uniqueString}_tag3", CreatedAt = DateTime.UtcNow },
                };
            context.Tags.AddRange(tags);
            context.SaveChanges();
            var tagEntities = context.Tags.ToArray();
            SampleExamContextHelper.SeededTags = tagEntities;
        }
    }
}
using System.Collections.Generic;

namespace SampleExam.Features.Tag
{

    public class TagsDTOEnvelope
    {
        public TagsDTOEnvelope(IEnumerable<TagDTO> tags, int tagCount)
        {
            this.Tags = tags;
            this.TagCount = tagCount;
        }

        public IEnumerable<TagDTO> Tags { get; }
        public int TagCount { get; }
    }
}
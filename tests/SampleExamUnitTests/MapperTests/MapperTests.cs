using AutoMapper;
using NUnit.Framework;
using SampleExam.Domain;
using SampleExam.Features.Tag;

namespace Tests
{
    public class MapperTests
    {
        private IMapper mapper;

        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg =>
           {
               cfg.AddMaps(typeof(SampleExam.Startup).Assembly);
           });
            this.mapper = configuration.CreateMapper();
        }

        [Test]
        public void ShouldMapTagToTagDTO()
        {
            var tag = new Tag() { Id = 3, Text = "Java" };
            var tagDto = mapper.Map<TagDTO>(tag);
            Assert.AreEqual(tagDto.Id, tag.Id);
            Assert.AreEqual(tagDto.Text, tag.Text);
        }
    }
}
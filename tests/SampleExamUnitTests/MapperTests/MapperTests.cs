using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using NUnit.Framework;
using SampleExam.Domain;
using SampleExam.Features.Exam;
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
            var dto = mapper.Map<TagDTO>(tag);
            Assert.AreEqual(dto.Id, tag.Id);
            Assert.AreEqual(dto.Text, tag.Text);
        }

        [Test]
        public void ShouldMapExamToExamDTO()
        {
            var tags = new List<ExamTag>(){
                    new ExamTag() { ExamId = 1, TagId = 1, Tag = new Tag() { Id = 1, Text = "Java"}},
                    new ExamTag() { ExamId = 1, TagId = 2, Tag = new Tag() { Id = 2, Text = "C#"}},
                    };
            var exam = new Exam()
            {
                Id = 1,
                UserId = 1,
                Title = "title",
                Description = "desc",
                TimeInMinutes = 55,
                PassPercentage = 65,
                IsPrivate = true,
                IsPublished = true,
                IsDeleted = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = DateTime.UtcNow,
                ExamTags = tags
            };

            var dto = mapper.Map<ExamDTO>(exam);
            Assert.AreEqual(dto.Id, exam.Id);
            Assert.AreEqual(dto.Title, exam.Title);

            Assert.AreEqual(dto.Description, exam.Description);
            Assert.AreEqual(dto.TimeInMinutes, exam.TimeInMinutes);
            Assert.AreEqual(dto.PassPercentage, exam.PassPercentage);
            Assert.AreEqual(dto.IsPrivate, exam.IsPrivate);
            Assert.AreEqual(dto.IsPublished, exam.IsPublished);
            Assert.AreEqual(dto.IsDeleted, exam.IsDeleted);
            Assert.AreEqual(dto.CreatedAt, exam.CreatedAt);
            Assert.AreEqual(dto.UpdatedAt, exam.UpdatedAt);

            foreach (var examTag in exam.ExamTags)
            {
                var tag = examTag.Tag;
                var count = dto.Tags.Where(t => t.Id == tag.Id && t.Text == tag.Text).Count();
                Assert.IsTrue(count == 1);
            }
        }



    }
}
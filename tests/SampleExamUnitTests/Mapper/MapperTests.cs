using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using NUnit.Framework;
using SampleExam.Domain;
using SampleExam.Features.Exam;
using SampleExam.Features.Tag;
using SampleExam.Features.User;

namespace Tests.Mapper
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
            var tag = new Tag() { TagId = "Java" };
            var dto = mapper.Map<TagDTO>(tag);
            Assert.AreEqual(dto.Tag, tag.TagId);
        }

        [Test]
        public void ShouldMapExamToExamDTO()
        {
            var exam = CreateExamEntity(true, true);
            var user = exam.User;
            var dto = mapper.Map<ExamDTO>(exam);
            AssertExam(dto, exam);
            AsserExamTags(dto, exam);
            var userDto = dto.User;
            AssertUser(userDto, user);
        }

        [Test]
        public void ShouldMapExamToExamDTWithoutIncludes()
        {
            var exam = CreateExamEntity(false, false);
            var user = exam.User;
            var dto = mapper.Map<ExamDTO>(exam);
            AssertExam(dto, exam);
            Assert.Zero(dto.Tags.Count);
            Assert.IsNull(dto.User);
        }
        [Test]
        public void ShouldMapExamDataToExam()
        {
            var data = new SampleExam.Features.Exam.Create.ExamData()
            {
                Title = "title",
                Description = "desc",
                TimeInMinutes = 10,
                PassPercentage = 55,
                IsPrivate = true
            };
            var exam = mapper.Map<Exam>(data);

        }

        private void AsserExamTags(ExamDTO dto, Exam exam)
        {
            foreach (var examTag in exam.ExamTags)
            {
                var tag = examTag.Tag;
                var count = dto.Tags.Where(t => t.Tag == tag.TagId).Count();
                Assert.IsTrue(count == 1);
            }
        }
        private void AssertExam(ExamDTO dto, Exam exam)
        {
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
        }

        private void AssertUser(UserDTO userDto, User user)
        {
            Assert.AreEqual(userDto.Id, user.Id);
            Assert.AreEqual(userDto.Firstname, user.Firstname);
            Assert.AreEqual(userDto.Lastname, user.Lastname);
            Assert.AreEqual(userDto.Middlename, user.Middlename);
            Assert.AreEqual(userDto.GenderId, user.GenderId);
            Assert.AreEqual(userDto.Dob, user.Dob);
            Assert.AreEqual(userDto.Email, user.Email);
        }
        private Exam CreateExamEntity(bool includeTags, bool includeUser)
        {
            var user = new User()
            {
                Id = 1,
                Firstname = "firstname",
                Lastname = "lastname",
                Middlename = "middlename",
                GenderId = 1,
                Dob = DateTime.UtcNow.AddYears(-20),
                Email = "example@example.com",
                Password = "asdadasd",
                IsEmailConfirmed = true,
                IsDeleted = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = DateTime.UtcNow
            };

            var tags = new List<ExamTag>(){
                    new ExamTag() { ExamId = 1, TagId = "Java", Tag = new Tag() {  TagId = "Java"}},
                    new ExamTag() { ExamId = 1, TagId = "C#", Tag = new Tag() {  TagId = "C#"}},
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

            };

            if (includeTags)
            {
                exam.ExamTags = tags;
            }

            if (includeTags)
            {
                exam.User = user;
            }

            return exam;
        }


    }
}
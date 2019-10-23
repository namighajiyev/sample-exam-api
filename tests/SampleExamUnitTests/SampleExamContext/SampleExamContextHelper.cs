using Data = SampleExam.Infrastructure.Data;
using SampleExam.Domain;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace SampleExamUnitTests.SampleExamContext
{
    public class SampleExamContextHelper
    {
        private readonly Data.SampleExamContext context;
        private ServiceProvider serviceProvider;

        public SampleExamContextHelper(Data.SampleExamContext context, ServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        public User AddNewUser()
        {
            var hasher = this.serviceProvider.GetRequiredService<IPasswordHasher<User>>();
            var now = DateTime.UtcNow;
            var user = new User()
            {
                Firstname = "fffff",
                Lastname = "llll",
                Middlename = "mmmmm",
                GenderId = 1,
                Dob = new DateTime(1986, 07, 04),
                Email = "namiq@example.com",
                Password = "qwertyu12",
                CreatedAt = now,
                UpdatedAt = now
            };
            user.Password = hasher.HashPassword(user, user.Password);
            this.context.Users.Add(user);
            this.context.SaveChanges();
            return user;
        }

        public Exam AddNewExam(int userId)
        {
            var exam = new Exam()
            {
                UserId = userId,

                Title = "ttt",

                Description = "ddd",

                TimeInMinutes = 55,

                PassPercentage = 50,

                IsPrivate = false,

                IsPublished = true,

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = DateTime.UtcNow
            };
            this.context.Exams.Add(exam);
            this.context.SaveChanges();
            return exam;
        }

        public UserExam AddUserExam(int userId, int examId)
        {
            var userExam = new UserExam()
            {
                ExamId = examId,
                UserId = userId,
                StartedtedAt = DateTime.UtcNow,
            };

            this.context.UserExams.Add(userExam);
            this.context.SaveChanges();
            return userExam;
        }
    }
}
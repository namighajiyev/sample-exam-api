using System;
using System.Collections.Generic;
using SampleExam.Infrastructure.Utils;

namespace SampleExamIntegrationTests.Helpers
{
    public static class TestData
    {
        public static class User
        {
            public static class Create
            {
                public static SampleExam.Features.User.Create.UserData NewUserData()
                {
                    var uniqueEmail = $"{Guid.NewGuid().ToGuidString()}@example.com";
                    var userData = new SampleExam.Features.User.Create.UserData()
                    {
                        Firstname = "Namig",
                        Lastname = "Hajiyev",
                        Middlename = "Zakir",
                        GenderId = 1,
                        Dob = new DateTime(1986, 04, 07),
                        Email = uniqueEmail,
                        Password = "2aEvJPCF",
                        ConfirmPassword = "2aEvJPCF"
                    };
                    return userData;
                }

            }
        }

        public static class Exam
        {
            public static class Create
            {
                public static SampleExam.Features.Exam.Create.ExamData NewExamData(
                     bool includeTags = true, bool isPrivate = false, string[] extraTags = null)
                {
                    var uniqueString = Guid.NewGuid().ToGuidString();
                    var random = new Random();
                    var examData = new SampleExam.Features.Exam.Create.ExamData()
                    {
                        Title = $"{uniqueString}_Title",

                        Description = $"{uniqueString}_Description",

                        TimeInMinutes = random.Next(30, 120),

                        PassPercentage = random.Next(50, 100),

                        IsPrivate = isPrivate,
                    };

                    if (includeTags)
                    {
                        var newTags = new List<string>() { $"{uniqueString}_tag1", $"{uniqueString}_tag2", $"{uniqueString}_tag3" };
                        if (extraTags != null)
                        {
                            newTags.AddRange(extraTags);
                        }
                        examData.Tags = newTags;
                    }
                    return examData;
                }
            }
        }
    }
}
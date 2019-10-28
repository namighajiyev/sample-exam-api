using System;

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
                    var uniqueEmail = $"{Guid.NewGuid().ToString().Replace("-", String.Empty)}@example.com";
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
    }
}
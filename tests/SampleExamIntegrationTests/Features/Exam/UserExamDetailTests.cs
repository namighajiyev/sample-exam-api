using System.Net.Http;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class UserExamDetailTests : IntegrationTestBase
    {
        public UserExamDetailTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldGetUserExamDetails()
        {

            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContext = this.dbContextFactory.CreateDbContext();

            //create private & public -user1
            var tuple = await httpCallHelper.CreateExam();
            var loggedUser1 = tuple.Item1;
            var examDto1 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam(true, true, null, loggedUser1);
            var examDtoPrivate1 = tuple.Item3;

            tuple = await httpCallHelper.CreateExam();
            var loggedUser2 = tuple.Item1;
            var examDto2 = tuple.Item3;
            tuple = await httpCallHelper.CreateExam(true, true, null, loggedUser2);
            var examDtoPrivate2 = tuple.Item3;

            //create private & public -user2
            var link1 = $"exams/user/exam/{examDto1.Id}";
            var link1IncludeUser = $"exams/user/exam/{examDto1.Id}?includeUser=true";
            var link1IncludeTags = $"exams/user/exam/{examDto1.Id}?includeTags=true";
            var link1IncludeUserAndTags = $"exams/user/exam/{examDto1.Id}?includeUser=true&includeTags=true";
            var link1Private = $"exams/user/exam/{examDtoPrivate1.Id}";
            var link1PrivateIncludeUser = $"exams/user/exam/{examDtoPrivate1.Id}?includeUser=true";
            var link1PrivateIncludeTags = $"exams/user/exam/{examDtoPrivate1.Id}?includeTags=true";
            var link1PrivateIncludeUserAndTags = $"exams/user/exam/{examDtoPrivate1.Id}?includeUser=true&includeTags=true";

            var link2 = $"exams/user/exam/{examDto2.Id}";

            //request unauthorized
            await client.GetUnauthorized(link1);

            //authorize user 1
            client.Authorize(loggedUser1.Token);

            //for public
            // include none
            var responseExam = await client.GetExamSuccesfully(link1);
            AssertHelper.AssertNoUserAndNoTagsIncluded(responseExam);
            //include user
            responseExam = await client.GetExamSuccesfully(link1IncludeUser);
            AssertHelper.AssertOnlyUserIncluded(responseExam);
            //include tags
            responseExam = await client.GetExamSuccesfully(link1IncludeTags);
            AssertHelper.AssertOnlyTagsIncluded(responseExam);
            //include both
            responseExam = await client.GetExamSuccesfully(link1IncludeUserAndTags);
            AssertHelper.AssertUserAndTagsIncluded(responseExam);

            //for private
            // include none
            responseExam = await client.GetExamSuccesfully(link1Private);
            AssertHelper.AssertNoUserAndNoTagsIncluded(responseExam);
            //include user
            responseExam = await client.GetExamSuccesfully(link1PrivateIncludeUser);
            AssertHelper.AssertOnlyUserIncluded(responseExam);
            //include tags
            responseExam = await client.GetExamSuccesfully(link1PrivateIncludeTags);
            AssertHelper.AssertOnlyTagsIncluded(responseExam);
            //include both
            responseExam = await client.GetExamSuccesfully(link1PrivateIncludeUserAndTags);
            AssertHelper.AssertUserAndTagsIncluded(responseExam);
            //request user 2 exam - should not found
            await client.GetNotFound(link2);

            //authorize user 2
            client.Authorize(loggedUser2.Token);
            //request user 1 exam - should not found
            await client.GetNotFound(link1);
        }
    }
}
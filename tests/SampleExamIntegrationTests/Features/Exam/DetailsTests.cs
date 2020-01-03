using System.Net.Http;
using SampleExam;
using SampleExam.Features.Exam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.Exam
{
    public class DetailsTests : IntegrationTestBase
    {
        public DetailsTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void ShouldGetExamDetails()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var dbContextHelper = new DbContextHelper(this.dbContextFactory);

            var tuple = await httpCallHelper.CreateExam();
            var examPublicDto = tuple.Item3;

            tuple = await httpCallHelper.CreateExam(true, true);
            var examPrivateDto = tuple.Item3;

            var getExamLink = $"exams/exam/{examPublicDto.Id}";
            var getExamLinkIncludeUser = $"exams/exam/{examPublicDto.Id}?includeUser=true";
            var getExamLinkIncludeTags = $"exams/exam/{examPublicDto.Id}?includeTags=true";
            var getExamLinkIncludeUserAndTags = $"exams/exam/{examPublicDto.Id}?includeUser=true&includeTags=true";

            var getPrivateExamLink = $"exams/exam/{examPrivateDto.Id}";

            //not published
            await client.GetNotFound(getExamLink);
            await client.GetNotFound(getPrivateExamLink);

            await dbContextHelper.PublishExamAsync(examPublicDto.Id);
            await dbContextHelper.PublishExamAsync(examPrivateDto.Id);

            //public and  published
            var responseExam = await client.GetExamSuccesfully(getExamLink);
            AssertHelper.AssertNoUserAndNoTagsIncluded(responseExam);

            //include user
            responseExam = await client.GetExamSuccesfully(getExamLinkIncludeUser);
            AssertHelper.AssertOnlyUserIncluded(responseExam);

            //include tags
            responseExam = await client.GetExamSuccesfully(getExamLinkIncludeTags);
            AssertHelper.AssertOnlyTagsIncluded(responseExam);

            //include user and tags
            responseExam = await client.GetExamSuccesfully(getExamLinkIncludeUserAndTags);
            AssertHelper.AssertUserAndTagsIncluded(responseExam);


            //private and  published
            await client.GetNotFound(getPrivateExamLink);
        }

    }
}
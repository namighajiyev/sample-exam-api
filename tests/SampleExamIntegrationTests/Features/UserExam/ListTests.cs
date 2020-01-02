using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleExam;
using SampleExamIntegrationTests.Helpers;
using Xunit;

namespace SampleExamIntegrationTests.Features.UserExam
{
    public class ListTests : IntegrationTestBase
    {
        public ListTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }

        [Fact]
        public async void UserExamListTests()
        {
            var client = httpClientFactory.CreateClient();
            var httpCallHelper = new HttpCallHelper(client);
            var user1 = (await httpCallHelper.CreateUserExam()).Item1;
            var user2 = (await httpCallHelper.CreateUserExam()).Item1;
            for (int i = 0; i < 3; i++)
            {
                await httpCallHelper.CreateUserExam(loggedUser: user1);
                await httpCallHelper.CreateUserExam(loggedUser: user2);
                await httpCallHelper.CreateUserExam(loggedUser: user1, isPrivate: true);
                await httpCallHelper.CreateUserExam(loggedUser: user2, isPrivate: true);

                //ended
                var userExamId = (await httpCallHelper.CreateUserExam(loggedUser: user1)).Item3.Id;
                client.Authorize(user1.Token);
                await httpCallHelper.EndUserExam(userExamId);

                userExamId = (await httpCallHelper.CreateUserExam(loggedUser: user1, isPrivate: true)).Item3.Id;
                client.Authorize(user1.Token);
                await httpCallHelper.EndUserExam(userExamId);

                userExamId = (await httpCallHelper.CreateUserExam(loggedUser: user2)).Item3.Id;
                client.Authorize(user2.Token);
                await httpCallHelper.EndUserExam(userExamId);

                userExamId = (await httpCallHelper.CreateUserExam(loggedUser: user2, isPrivate: true)).Item3.Id;
                client.Authorize(user2.Token);
                await httpCallHelper.EndUserExam(userExamId);

            }

            client.Unauthorize();

            var link = "/userexams";
            var linkIncludeExams = "/userexams?includeExams=true";
            await client.GetUnauthorized(link);

            client.Authorize(user1.Token);

            var envelope = await client.GetUserExamsEnvelopeSuccesfully(link);
            Assert.Equal(envelope.UserExamCount, envelope.UserExams.Count());
            Assert.Equal(envelope.UserExamCount, 13);
            foreach (var userExam in envelope.UserExams)
            {
                Assert.Null(userExam.Exam);
                Assert.Equal(userExam.UserId, user1.Id);
            }

            client.Authorize(user2.Token);

            envelope = await client.GetUserExamsEnvelopeSuccesfully(link);
            Assert.Equal(envelope.UserExamCount, envelope.UserExams.Count());
            Assert.Equal(envelope.UserExamCount, 13);
            foreach (var userExam in envelope.UserExams)
            {
                Assert.Null(userExam.Exam);
                Assert.Equal(userExam.UserId, user2.Id);
            }
            envelope = await client.GetUserExamsEnvelopeSuccesfully(linkIncludeExams);
            Assert.Equal(envelope.UserExamCount, envelope.UserExams.Count());
            Assert.Equal(envelope.UserExamCount, 13);
            foreach (var userExam in envelope.UserExams)
            {
                Assert.NotNull(userExam.Exam);
                Assert.Equal(userExam.UserId, user2.Id);
            }
            var limitOffsetTester = new LimitOffsetTester(client, link);
            await limitOffsetTester.DoTest(GetUserExams);

        }

        private async Task<Tuple<IEnumerable<SampleExam.Features.UserExam.UserExamDTO>, int>> GetUserExams(System.Net.Http.HttpClient client, string link)
        {
            var envelope = await client.GetUserExamsEnvelopeSuccesfully(link);
            return Tuple.Create(envelope.UserExams, envelope.UserExamCount);
        }


        //create with user 1 - 3 public exam and end them
        //create with user 1 - 2 public exam and dont end them
        //create with user 1 - 3 private exam and end them
        //create with user 1 - 2 private exam and dont end them

        //create with user 2 - 1 public exam and end them
        //create with user 2 - 1 public exam and dont end them
        //create with user 2 - 1 private exam and end them
        //create with user 2 - 1 private exam and dont end them

        //authorize user 1
        //get should return 6 user exams
        //get with exam included should be 6
        // test limit ofset ....
    }
}
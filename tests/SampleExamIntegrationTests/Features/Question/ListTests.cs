using SampleExam;

namespace SampleExamIntegrationTests.Features.Question
{
    public class ListTests : IntegrationTestBase
    {
        public ListTests(
            CustomWebApplicationFactory<Startup> factory,
            DbContextFactory dbContextFactory
        ) : base(factory, dbContextFactory)
        {

        }


        //GET /questions

        //create public published exams
        // create private published exams
        //create public NOT published exams
        // create private NOT published exams

        // call methode for all cases make sure only first case returns OK (published and public)
        // test includeAnswerOptions
        // limit offset

    }
}
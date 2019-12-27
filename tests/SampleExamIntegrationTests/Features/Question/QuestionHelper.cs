using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Question;
using SampleExamIntegrationTests.Helpers;

namespace SampleExamIntegrationTests.Features.Question
{
    public class QuestionHelper
    {
        public static async Task<Tuple<IEnumerable<QuestionDTO>, int>> GetQuestions(HttpClient client, string link)
        {
            var envelope = await client.GetQuestionsSuccesfully(link);
            return Tuple.Create(envelope.Questions, envelope.QuestionCount);
        }
    }
}
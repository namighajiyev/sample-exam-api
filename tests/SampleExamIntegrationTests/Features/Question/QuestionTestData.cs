using System;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Auth;
using SampleExam.Features.Exam;
using SampleExam.Features.Question;
using SampleExamIntegrationTests.Helpers;

namespace SampleExamIntegrationTests.Features.Question
{
    public class QuestionTestData
    {
        public QuestionTestData(System.Net.Http.HttpClient client)
        {
            CreateQuestions(client).Wait();
        }

        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u1PublicNotPublished { get; private set; }
        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u1PublicPublished { get; private set; }
        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u1PrivateNotPublished { get; private set; }
        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u1PrivatePublished { get; private set; }
        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u2PublicNotPublished { get; private set; }
        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u2PublicPublished { get; private set; }
        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u2PrivateNotPublished { get; private set; }
        public Tuple<LoginUserDTO, SampleExam.Features.Exam.Create.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, QuestionDTO> u2PrivatePublished { get; private set; }

        private async Task CreateQuestions(HttpClient client)
        {
            var httpCallHelper = new HttpCallHelper(client);
            //u means user
            u1PublicNotPublished = await httpCallHelper.CreateQuestion();
            u1PublicPublished = await httpCallHelper.CreateQuestion(loggedUser: u1PublicNotPublished.Item1);
            u1PrivateNotPublished = await httpCallHelper.CreateQuestion(loggedUser: u1PublicNotPublished.Item1, isPrivate: true);
            u1PrivatePublished = await httpCallHelper.CreateQuestion(loggedUser: u1PublicNotPublished.Item1, isPrivate: true);

            u2PublicNotPublished = await httpCallHelper.CreateQuestion();
            u2PublicPublished = await httpCallHelper.CreateQuestion(loggedUser: u2PublicNotPublished.Item1);
            u2PrivateNotPublished = await httpCallHelper.CreateQuestion(loggedUser: u2PublicNotPublished.Item1, isPrivate: true);
            u2PrivatePublished = await httpCallHelper.CreateQuestion(loggedUser: u2PublicNotPublished.Item1, isPrivate: true);
            //publishing exams
            client.Authorize(u1PublicNotPublished.Item1.Token);
            await httpCallHelper.PublishExam(u1PublicPublished.Item3.Id);
            await httpCallHelper.PublishExam(u1PrivatePublished.Item3.Id);

            client.Authorize(u2PublicNotPublished.Item1.Token);
            await httpCallHelper.PublishExam(u2PublicPublished.Item3.Id);
            await httpCallHelper.PublishExam(u2PrivatePublished.Item3.Id);
            client.Unauthorize();
        }
    }
}
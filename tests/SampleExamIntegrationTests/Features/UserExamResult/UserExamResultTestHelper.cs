using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleExam.Infrastructure.Data;
using SampleExamIntegrationTests.Helpers;
using Xunit;
using static SampleExam.Features.QuestionAnswer.CreateOrUpdate;

namespace SampleExamIntegrationTests.Features.UserExamResult
{
    public class UserExamResultTestHelper
    {
        public enum AnswerType
        {
            Right = 1,
            Wrong = 2,
            Random = 3
        }

        private readonly HttpClient client;
        private readonly DbContextFactory dbContextFactory;

        public UserExamResultTestHelper(System.Net.Http.HttpClient client, DbContextFactory dbContextFactory)
        {
            this.client = client;
            this.dbContextFactory = dbContextFactory;
        }


        public async Task<Tuple<int, int>> TakeExam(int userExamId, bool fail = false, bool skipSomeQuestions = false)
        {
            var link = "/questionanswers";
            SampleExam.Domain.UserExam userExam = null;
            using (var context = dbContextFactory.CreateDbContext())
            {
                userExam = context.UserExams
                .Include(e => e.Exam)
                .ThenInclude(e => e.Questions).ThenInclude(e => e.AnswerOptions)
                .Where(e => e.Id == userExamId).First();
            }
            var questions = userExam.Exam.Questions.Shuffle().ToArray();
            int rightAnswerCount = questions.Length * userExam.Exam.PassPercentage / 100;
            rightAnswerCount = fail ? Math.Max(0, rightAnswerCount - 2) : Math.Min(questions.Length, rightAnswerCount + 2);


            if (skipSomeQuestions)
            {
                var takePersentage = userExam.Exam.PassPercentage + ((100 - userExam.Exam.PassPercentage) / 2);
                var take = questions.Count() * takePersentage / 100;
                take = Math.Max(take, rightAnswerCount);
                questions = questions.Take(take).ToArray();
            }

            Assert.True(questions.Count() > rightAnswerCount);

            var request = new Request();
            request.UserExamQuestionAnswer = new UserExamQuestionAnswerData() { UserExamId = userExamId };

            //answering questions randomly
            foreach (var question in questions)
            {
                request.UserExamQuestionAnswer.QuestionId = question.Id;
                request.UserExamQuestionAnswer.AnswerOptionIds = AnswerQuestion(question, AnswerType.Random);
                var questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            }

            //changing answers to fail/succeed
            var random = Utils.NewRandom();
            for (int i = 0; i < questions.Length; i++)
            {
                var question = questions[i];
                request.UserExamQuestionAnswer.QuestionId = question.Id;
                var answerType = i < rightAnswerCount ? AnswerType.Right : AnswerType.Wrong;
                request.UserExamQuestionAnswer.AnswerOptionIds = AnswerQuestion(question, answerType);
                var questionAnswer = await client.PostQuestionAnswerSuccesfully(link, request);
            }
            return Tuple.Create(rightAnswerCount, questions.Count());

        }

        private IEnumerable<int> AnswerQuestion(SampleExam.Domain.Question question, AnswerType answerType)
        {
            var rightAnswerCount = question.AnswerOptions.Where(e => e.IsRight).Count();
            var isRadio = question.QuestionTypeId == SeedData.QuestionTypes.Radio.Id;
            var random = Utils.NewRandom();
            var answerOptionCount = isRadio ? 1 : random.Next(1, rightAnswerCount);
            var answerOptions = question.AnswerOptions.Shuffle().ToArray();
            switch (answerType)
            {
                case AnswerType.Right:
                    return answerOptions.Where(e => e.IsRight).Select(e => e.Id).ToArray();
                case AnswerType.Wrong:
                    var rightAnswer = answerOptions.Where(e => e.IsRight).First();
                    return answerOptions.Where(e => e.Id != rightAnswer.Id).Select(e => e.Id).Take(answerOptionCount).ToArray();
                case AnswerType.Random:
                default:
                    return answerOptions.Select(e => e.Id).Take(answerOptionCount).ToArray();

            }

        }

    }
}
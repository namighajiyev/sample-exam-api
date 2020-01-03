using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Auth;
using SampleExam.Features.Exam;
using SampleExam.Features.Question;
using SampleExam.Features.UserExam;
using SampleExam.Infrastructure.Data;
using static SampleExam.Features.QuestionAnswer.CreateOrUpdate;

namespace SampleExamIntegrationTests.Helpers.Data
{
    public class UserQuestionAnswerData
    {
        public LoginUserDTO User { get; set; }
        public ExamDTO Exam { get; set; }
        public UserExamDTO UserExam { get; set; }

        public IEnumerable<QuestionDTO> Questions { get; set; }
        public int QuestionCount { get; set; }

        public QuestionDTO RadioQuestion1 { get; set; }
        public QuestionDTO RadioQuestion2 { get; set; }

        public QuestionDTO CheckboxQuestion1 { get; set; }
        public QuestionDTO CheckboxQuestion2 { get; set; }

        public UserExamQuestionAnswerData RadioQuestion1Answer { get; set; }
        public UserExamQuestionAnswerData RadioQuestion2Answer { get; set; }

        public UserExamQuestionAnswerData CheckboxQuestion1Answer { get; set; }
        public UserExamQuestionAnswerData CheckboxQuestion2Answer { get; set; }

        public int[] RadioQuestion1AnswerIds { get; set; }
        public int[] RadioQuestion2AnswerIds { get; set; }

        public int[] CheckboxQuestion1AnswerIds { get; set; }
        public int[] CheckboxQuestion2AnswerIds { get; set; }

        private Request CreateAnswerRequest(UserExamQuestionAnswerData answerData, bool isRadio, int[] answerOptionIds, bool randomTake)
        {
            var request = new Request()
            {
                UserExamQuestionAnswer = new UserExamQuestionAnswerData()
                {
                    UserExamId = answerData.UserExamId,
                    QuestionId = answerData.QuestionId
                }
            };
            if (randomTake)
            {
                var rnd = new Random(Guid.NewGuid().GetHashCode());
                answerOptionIds = answerOptionIds.OrderBy(x => rnd.Next()).ToArray();
                answerOptionIds = isRadio ? new int[] { answerOptionIds[0] } : new int[] { answerOptionIds[0], answerOptionIds[1] };
            }
            request.UserExamQuestionAnswer.AnswerOptionIds = answerOptionIds;
            return request;
        }

        public Request CreateRadioQuestion1AnswerRequest(int? notThisId = null, int[] answerOptionIds = null)
        {
            var randomTake = answerOptionIds == null;
            answerOptionIds = answerOptionIds ?? (notThisId.HasValue ?
            this.RadioQuestion1AnswerIds.Where(e => e != notThisId).ToArray() : this.RadioQuestion1AnswerIds);
            return CreateAnswerRequest(this.RadioQuestion1Answer, true, answerOptionIds, randomTake);
        }

        public Request CreateRadioQuestion2AnswerRequest(int? notThisId = null, int[] answerOptionIds = null)
        {
            var randomTake = answerOptionIds == null;
            answerOptionIds = answerOptionIds ?? (notThisId.HasValue ?
            this.RadioQuestion2AnswerIds.Where(e => e != notThisId).ToArray() : this.RadioQuestion2AnswerIds);
            return CreateAnswerRequest(this.RadioQuestion2Answer, true, answerOptionIds, randomTake);
        }

        public Request CreateCheckboxQuestion1AnswerRequest(int? notThisId = null, int[] answerOptionIds = null)
        {
            var randomTake = answerOptionIds == null;
            answerOptionIds = answerOptionIds ?? (notThisId.HasValue ?
            this.CheckboxQuestion1AnswerIds.Where(e => e != notThisId).ToArray() : this.CheckboxQuestion1AnswerIds);
            return CreateAnswerRequest(this.CheckboxQuestion1Answer, false, answerOptionIds, randomTake);
        }
        public Request CreateCheckboxQuestion2AnswerRequest(int? notThisId = null, int[] answerOptionIds = null)
        {
            var randomTake = answerOptionIds == null;
            answerOptionIds = answerOptionIds ?? (notThisId.HasValue ?
            this.CheckboxQuestion2AnswerIds.Where(e => e != notThisId).ToArray() : this.CheckboxQuestion2AnswerIds);
            return CreateAnswerRequest(this.CheckboxQuestion2Answer, false, answerOptionIds, randomTake);
        }

    }

    public class QuestionAnswerTestData
    {
        private readonly HttpClient client;
        public UserQuestionAnswerData User1 { get; private set; }
        public UserQuestionAnswerData User2 { get; private set; }
        public UserQuestionAnswerData User3 { get; private set; }
        public UserQuestionAnswerData User4 { get; private set; }
        public UserQuestionAnswerData User5 { get; private set; }
        public Request NonExistingRequest { get; private set; }

        public QuestionAnswerTestData(System.Net.Http.HttpClient client)
        {
            this.client = client;
            this.User1 = new UserQuestionAnswerData();
            this.User2 = new UserQuestionAnswerData();
            this.NonExistingRequest = new SampleExam.Features.QuestionAnswer.CreateOrUpdate.Request()
            {
                UserExamQuestionAnswer = new UserExamQuestionAnswerData()
                { UserExamId = int.MaxValue, QuestionId = int.MaxValue, AnswerOptionIds = new int[] { 1, 2 } }
            };
            CreateData().Wait();
        }

        private async Task CreateData()
        {
            User1 = await CreateData(client);
            User2 = await CreateData(client);
            User3 = await CreateData(client);
            User4 = await CreateData(client);
            User5 = await CreateData(client);
        }
        public static async Task<UserQuestionAnswerData> CreateData(System.Net.Http.HttpClient client)
        {
            var httpCallHelper = new HttpCallHelper(client);
            var user = new UserQuestionAnswerData();
            var userData = await httpCallHelper.CreateUserExam();
            user.User = userData.Item1;
            user.Exam = userData.Item2;
            user.UserExam = userData.Item3;

            var questions = await httpCallHelper.GetQuestions(userData.Item2.Id);
            user.Questions = questions.Questions;
            user.QuestionCount = questions.QuestionCount;

            user.RadioQuestion1 = questions.Questions.Where(e => e.QuestionTypeId == SeedData.QuestionTypes.Radio.Id).First();
            user.RadioQuestion2 = questions.Questions.Where(e => e.QuestionTypeId == SeedData.QuestionTypes.Radio.Id && e.Id != user.RadioQuestion1.Id).First();
            user.CheckboxQuestion1 = questions.Questions.Where(e => e.QuestionTypeId == SeedData.QuestionTypes.Checkbox.Id).First();
            user.CheckboxQuestion2 = questions.Questions.Where(e => e.QuestionTypeId == SeedData.QuestionTypes.Checkbox.Id && e.Id != user.CheckboxQuestion1.Id).First();

            user.RadioQuestion1Answer = new UserExamQuestionAnswerData() { UserExamId = userData.Item3.Id, QuestionId = user.RadioQuestion1.Id };
            user.RadioQuestion2Answer = new UserExamQuestionAnswerData() { UserExamId = userData.Item3.Id, QuestionId = user.RadioQuestion2.Id };
            user.CheckboxQuestion1Answer = new UserExamQuestionAnswerData() { UserExamId = userData.Item3.Id, QuestionId = user.CheckboxQuestion1.Id };
            user.CheckboxQuestion2Answer = new UserExamQuestionAnswerData() { UserExamId = userData.Item3.Id, QuestionId = user.CheckboxQuestion2.Id };

            user.RadioQuestion1AnswerIds = user.RadioQuestion1.AnswerOptions.Select(e => e.Id).ToArray();
            user.RadioQuestion2AnswerIds = user.RadioQuestion2.AnswerOptions.Select(e => e.Id).ToArray();
            user.CheckboxQuestion1AnswerIds = user.CheckboxQuestion1.AnswerOptions.Select(e => e.Id).ToArray();
            user.CheckboxQuestion2AnswerIds = user.CheckboxQuestion2.AnswerOptions.Select(e => e.Id).ToArray();
            return user;

        }
    }
}
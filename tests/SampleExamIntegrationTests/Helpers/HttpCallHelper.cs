using System;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Auth;
using SampleExam.Features.Exam;
using SampleExam.Features.User;
using UserCreate = SampleExam.Features.User.Create;
using ExamCreate = SampleExam.Features.Exam.Create;

namespace SampleExamIntegrationTests.Helpers
{
    public class HttpCallHelper
    {
        private readonly HttpClient client;

        public HttpCallHelper(HttpClient client)
        {
            this.client = client;
        }
        public async Task<Tuple<UserCreate.UserData, UserDTO>> CreateUser()
        {
            var userData = TestData.User.Create.NewUserData();
            var responseUser = await client.PostUserSuccesfully("/users", new UserCreate.Request() { User = userData });
            return Tuple.Create(userData, responseUser);
        }


        public async Task<Tuple<UserCreate.UserData, UserDTO, Login.UserData, LoginUserDTO>> CreateUserAndLogin()
        {
            var user = await this.CreateUser();
            var userData = user.Item1;
            var loginUser = new Login.UserData() { Email = userData.Email, Password = userData.Password };
            var loggedUser = await client.PostLoginSucessfully("/auth/login", new Login.Request() { User = loginUser });
            return Tuple.Create(user.Item1, user.Item2, loginUser, loggedUser);
        }

        public async Task<Tuple<LoginUserDTO, ExamCreate.ExamData, ExamDTO>> CreateExam(
            bool includeTags = true, bool isPrivate = false, string[] extraTags = null,
            LoginUserDTO loggedUser = null)
        {
            loggedUser = loggedUser ?? (await CreateUserAndLogin()).Item4;
            var examData = TestData.Exam.Create.NewExamData(includeTags, isPrivate, extraTags);
            client.Authorize(loggedUser.Token);
            var responseExam = await client.PostExamSuccesfully("/exams", new ExamCreate.Request() { Exam = examData });
            client.Unauthorize();
            return Tuple.Create(loggedUser, examData, responseExam);

        }

        public async Task<Tuple<LoginUserDTO, ExamDTO>> CreatePublishedExam(
            bool includeTags = true, bool isPrivate = false, string[] extraTags = null, LoginUserDTO loggedUser = null)
        {
            var tuple = await CreateExam(includeTags, isPrivate, extraTags, loggedUser);
            var examDto = tuple.Item3;
            var user = tuple.Item1;
            var link = $"exams/publish/{examDto.Id}";
            client.Authorize(user.Token);
            var responseExam = await client.PutExamSuccesfully(link, null);
            client.Unauthorize();
            return Tuple.Create(user, responseExam);
        }

        public async Task<ExamDTO> PublishExam(int examId)
        {
            var link = $"exams/publish/{examId}";
            var responseExam = await client.PutExamSuccesfully(link, null);
            return responseExam;
        }

        public async Task<Tuple<LoginUserDTO, ExamCreate.ExamData, ExamDTO, SampleExam.Features.Question.Create.QuestionData, SampleExam.Features.Question.QuestionDTO>> CreateQuestion(
            bool includeTags = true,
        bool isPrivate = false, string[] extraTags = null,
                        LoginUserDTO loggedUser = null, bool isRadio = true)
        {
            var examItems = await CreateExam(includeTags, isPrivate, extraTags, loggedUser);
            var questionData = TestData.Question.Create.NewQuestionData(isRadio);
            client.Authorize(examItems.Item1.Token);
            var link = $"/questions/{examItems.Item3.Id}";
            var envelope = await client.PostSucessfully<SampleExam.Features.Question.QuestionDTOEnvelope>(link, new SampleExam.Features.Question.Create.Request() { Question = questionData });
            client.Unauthorize();
            return Tuple.Create(examItems.Item1, examItems.Item2, examItems.Item3, questionData, envelope.Question);
        }
    }
}
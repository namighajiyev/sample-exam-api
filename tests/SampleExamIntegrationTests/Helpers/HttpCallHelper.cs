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
            var response = await client.PostAsJsonAsync<UserCreate.Request>("/users", new UserCreate.Request() { User = userData });
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<UserDTOEnvelope>();
            var responseUser = envelope.User;
            return Tuple.Create(userData, responseUser);
        }


        public async Task<Tuple<UserCreate.UserData, UserDTO, Login.UserData, LoginUserDTO>> CreateUserAndLogin()
        {
            var user = await this.CreateUser();
            var userData = user.Item1;
            var loginUser = new Login.UserData() { Email = userData.Email, Password = userData.Password };
            var response = await client.PostAsJsonAsync<Login.Request>("/auth/login", new Login.Request() { User = loginUser });
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<LoginUserDTOEnvelope>();
            var loggedUser = envelope.User;
            return Tuple.Create(user.Item1, user.Item2, loginUser, loggedUser);
        }

        public async Task<Tuple<LoginUserDTO, ExamCreate.ExamData, ExamDTO>> CreateExam(
            bool includeTags = true, bool isPrivate = false, string[] extraTags = null,
            LoginUserDTO loggedUser = null)
        {
            loggedUser = loggedUser ?? (await CreateUserAndLogin()).Item4;
            var examData = TestData.Exam.Create.NewExamData(includeTags, isPrivate, extraTags);
            client.Authorize(loggedUser.Token);
            var response = await client.PostAsJsonAsync<ExamCreate.Request>("/exams", new ExamCreate.Request() { Exam = examData });
            response.EnsureSuccessStatusCode();
            client.Unauthorize();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
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
            var response = await client.PutAsync(link, null);
            response.EnsureSuccessStatusCode();
            client.Unauthorize();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            return Tuple.Create(user, responseExam);
        }

        public async Task<ExamDTO> PublishExam(int examId)
        {
            var link = $"exams/publish/{examId}";
            var response = await client.PutAsync(link, null);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            return responseExam;
        }

    }
}
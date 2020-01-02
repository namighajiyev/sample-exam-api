using System;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Auth;
using SampleExam.Features.Exam;
using SampleExam.Features.UserExam;

namespace SampleExamIntegrationTests.Helpers.Data
{
    public class UserExamData
    {
        private readonly HttpClient client;

        public UserExamData(System.Net.Http.HttpClient client)
        {
            this.client = client;
            CreateUserExams().Wait();
        }

        public Tuple<LoginUserDTO, ExamDTO, UserExamDTO> User1PublicUserExam { get; private set; }
        public Tuple<LoginUserDTO, ExamDTO, UserExamDTO> User1PublicUserExam2 { get; private set; }
        public Tuple<LoginUserDTO, ExamDTO, UserExamDTO> User1PrivateUserExam { get; private set; }
        public Tuple<LoginUserDTO, ExamDTO, UserExamDTO> User2PublicUserExam { get; private set; }
        public Tuple<LoginUserDTO, ExamDTO, UserExamDTO> User2PublicUserExam2 { get; private set; }
        public Tuple<LoginUserDTO, ExamDTO, UserExamDTO> User2PrivateUserExam { get; private set; }

        private async Task CreateUserExams()
        {
            var httpCallHelper = new HttpCallHelper(client);
            this.User1PublicUserExam = await httpCallHelper.CreateUserExam();
            this.User1PublicUserExam2 = await httpCallHelper.CreateUserExam(loggedUser: User1PublicUserExam.Item1);
            this.User1PrivateUserExam = await httpCallHelper.CreateUserExam(loggedUser: User1PublicUserExam.Item1, isPrivate: true);

            this.User2PublicUserExam = await httpCallHelper.CreateUserExam();
            this.User2PublicUserExam2 = await httpCallHelper.CreateUserExam(loggedUser: User2PublicUserExam.Item1);
            this.User2PrivateUserExam = await httpCallHelper.CreateUserExam(loggedUser: User2PublicUserExam.Item1, isPrivate: true);
        }


    }
}
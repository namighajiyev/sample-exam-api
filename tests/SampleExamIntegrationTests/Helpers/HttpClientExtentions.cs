using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Exam;
using SampleExam.Features.Question;
using SampleExam.Features.User;
using SampleExam.Infrastructure.Errors;

namespace SampleExamIntegrationTests.Helpers
{
    public static class HttpClientExtentions
    {
        private const string AUTH_HEADER_KEY = "Authorization";
        private const string AUTH_SCHEMA = "Bearer";

        public static bool IsAuthorized(this HttpClient client)
        {
            return client.DefaultRequestHeaders.Contains(AUTH_HEADER_KEY);
        }

        public static void Authorize(this HttpClient client, string token)
        {
            if (client.IsAuthorized())
            {
                Unauthorize(client);
            }
            client.DefaultRequestHeaders.Add(AUTH_HEADER_KEY, $"{AUTH_SCHEMA} {token}");
        }

        public static void Unauthorize(this HttpClient client)
        {
            if (client.IsAuthorized())
            {
                client.DefaultRequestHeaders.Remove(AUTH_HEADER_KEY);
            }
        }

        public static async Task<ApiProblemDetails> GetUnauthorized(this HttpClient client, string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
            return problemDetails;
        }

        public static async Task<ExamDTO> GetExamSuccesfully(this HttpClient client, string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            return responseExam;
        }

        public static async Task<QuestionDTO> GetQuestionSuccesfully(this HttpClient client, string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<QuestionDTOEnvelope>();
            var responseExam = envelope.Question;
            return responseExam;
        }

        public static async Task<QuestionsDTOEnvelope> GetQuestionsSuccesfully(this HttpClient client, string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<QuestionsDTOEnvelope>();
            return envelope;
        }

        public static async Task<IEnumerable<ExamDTO>> GetExamsSuccesfully(this HttpClient client, string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamsDTOEnvelope>();
            var responseExam = envelope.Exams;
            return responseExam;
        }
        public static async Task<ExamsDTOEnvelope> GetExamsEnvelopeSuccesfully(this HttpClient client, string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamsDTOEnvelope>();
            return envelope;
        }

        public static async Task PostSucessfully(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
        }

        public static async Task<T> PostSucessfully<T>(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<T>();
            return envelope;
        }

        public static async Task PostNotFound(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureNotFoundStatusCode();
        }
        public static async Task<ExamDTO> DeleteExamSucessfully(this HttpClient client, string link)
        {
            var response = await client.DeleteAsync(link);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            return responseExam;
        }

        public static async Task DeleteSucessfully(this HttpClient client, string link)
        {
            var response = await client.DeleteAsync(link);
            response.EnsureSuccessStatusCode();
        }

        public static async Task<ApiProblemDetails> PostUnauthorized(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
            return problemDetails;
        }

        public static async Task<ApiProblemDetails> DeleteUnauthorized(this HttpClient client, string link)
        {
            var response = await client.DeleteAsync(link);
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
            return problemDetails;
        }


        public static async Task<ApiProblemDetails> PostBadRequest(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureBadRequestStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
            return problemDetails;
        }
        public static async Task<SampleExam.Features.Auth.LoginUserDTO> PostLoginSucessfully(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<SampleExam.Features.Auth.LoginUserDTOEnvelope>();
            var user = envelope.User;
            return user;
        }

        public static async Task<ExamDTO> PutExamSuccesfully(this HttpClient client, string link, object data)
        {
            var response = await client.PutAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            return responseExam;
        }

        public static async Task PutSucessfully(this HttpClient client, string link, object data)
        {
            var response = await client.PutAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
        }
        public static async Task<QuestionDTO> PutQuestionSucessfully(this HttpClient client, string link, object data)
        {
            var response = await client.PutAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<QuestionDTOEnvelope>();
            var responseExam = envelope.Question;
            return responseExam;
        }
        public static async Task<ApiProblemDetails> PutUnauthorized(this HttpClient client, string link, object data)
        {
            var response = await client.PutAsJsonAsync<object>(link, data);
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<ApiProblemDetails>();
            return problemDetails;
        }

        public static async Task<ApiProblemDetails> PutBadRequest(this HttpClient client, string link, object data)
        {
            var response = await client.PutAsJsonAsync<object>(link, data);
            response.EnsureBadRequestStatusCode();
            var problemDetails = await response.Content.ReadAsAsync<SampleExam.Infrastructure.Errors.ApiProblemDetails>();
            return problemDetails;
        }
        public static async Task<ExamDTO> PostExamSuccesfully(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<ExamDTOEnvelope>();
            var responseExam = envelope.Exam;
            return responseExam;
        }
        public static async Task<UserDTO> PostUserSuccesfully(this HttpClient client, string link, object data)
        {
            var response = await client.PostAsJsonAsync<object>(link, data);
            response.EnsureSuccessStatusCode();
            var envelope = await response.Content.ReadAsAsync<UserDTOEnvelope>();
            var responseUser = envelope.User;
            return responseUser;
        }

        public static async Task GetNotFound(this HttpClient client, string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureNotFoundStatusCode();
        }

        public static async Task DeleteNotFound(this HttpClient client, string link)
        {
            var response = await client.DeleteAsync(link);
            response.EnsureNotFoundStatusCode();
        }
        public static async Task PutNotFound(this HttpClient client, string link, object data)
        {
            var response = await client.PutAsJsonAsync<object>(link, data);
            response.EnsureNotFoundStatusCode();
        }
    }
}
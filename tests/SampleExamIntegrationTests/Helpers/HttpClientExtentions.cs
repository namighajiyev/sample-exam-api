using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Exam;
using SampleExam.Features.User;

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

        //methods are not async/await beacuse MaxConcurrencySyncContext error
        //todo use async/await after bug fix

        public static SampleExam.Infrastructure.Errors.ApiProblemDetails GetUnauthorized(this HttpClient client, string link)
        {
            var response = client.GetAsync(link).Result;
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = response.Content.ReadAsAsync<SampleExam.Infrastructure.Errors.ApiProblemDetails>().Result;
            return problemDetails;
        }

        public static ExamDTO GetExamSuccesfully(this HttpClient client, string link)
        {
            var response = client.GetAsync(link).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<ExamDTOEnvelope>().Result;
            var responseExam = envelope.Exam;
            return responseExam;
        }

        public static IEnumerable<ExamDTO> GetExamsSuccesfully(this HttpClient client, string link)
        {
            var response = client.GetAsync(link).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<ExamsDTOEnvelope>().Result;
            var responseExam = envelope.Exams;
            return responseExam;
        }
        public static ExamsDTOEnvelope GetExamsEnvelopeSuccesfully(this HttpClient client, string link)
        {
            var response = client.GetAsync(link).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<ExamsDTOEnvelope>().Result;
            return envelope;
        }

        public static void PostSucessfully(this HttpClient client, string link, object data)
        {
            var response = client.PostAsJsonAsync<object>(link, data).Result;
            response.EnsureSuccessStatusCode();
        }

        public static ExamDTO DeleteExamSucessfully(this HttpClient client, string link)
        {
            var response = client.DeleteAsync(link).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<ExamDTOEnvelope>().Result;
            var responseExam = envelope.Exam;
            return responseExam;
        }

        public static void DeleteSucessfully(this HttpClient client, string link)
        {
            var response = client.DeleteAsync(link).Result;
            response.EnsureSuccessStatusCode();
        }

        public static SampleExam.Infrastructure.Errors.ApiProblemDetails PostUnauthorized(this HttpClient client, string link, object data)
        {
            var response = client.PostAsJsonAsync<object>(link, data).Result;
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = response.Content.ReadAsAsync<SampleExam.Infrastructure.Errors.ApiProblemDetails>().Result;
            return problemDetails;
        }

        public static SampleExam.Infrastructure.Errors.ApiProblemDetails DeleteUnauthorized(this HttpClient client, string link)
        {
            var response = client.DeleteAsync(link).Result;
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = response.Content.ReadAsAsync<SampleExam.Infrastructure.Errors.ApiProblemDetails>().Result;
            return problemDetails;
        }


        public static SampleExam.Infrastructure.Errors.ApiProblemDetails PostBadRequest(this HttpClient client, string link, object data)
        {
            var response = client.PostAsJsonAsync<object>(link, data).Result;
            response.EnsureBadRequestStatusCode();
            var problemDetails = response.Content.ReadAsAsync<SampleExam.Infrastructure.Errors.ApiProblemDetails>().Result;
            return problemDetails;
        }
        public static SampleExam.Features.Auth.LoginUserDTO PostLoginSucessfully(this HttpClient client, string link, object data)
        {
            var response = client.PostAsJsonAsync<object>(link, data).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<SampleExam.Features.Auth.LoginUserDTOEnvelope>().Result;
            var user = envelope.User;
            return user;
        }

        public static ExamDTO PutExamSuccesfully(this HttpClient client, string link, object data)
        {
            var response = client.PutAsJsonAsync<object>(link, data).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<ExamDTOEnvelope>().Result;
            var responseExam = envelope.Exam;
            return responseExam;
        }

        public static void PutSucessfully(this HttpClient client, string link, object data)
        {
            var response = client.PutAsJsonAsync<object>(link, data).Result;
            response.EnsureSuccessStatusCode();
        }

        public static SampleExam.Infrastructure.Errors.ApiProblemDetails PutUnauthorized(this HttpClient client, string link, object data)
        {
            var response = client.PutAsJsonAsync<object>(link, data).Result;
            response.EnsureUnauthorizedStatusCode();
            var problemDetails = response.Content.ReadAsAsync<SampleExam.Infrastructure.Errors.ApiProblemDetails>().Result;
            return problemDetails;
        }

        public static SampleExam.Infrastructure.Errors.ApiProblemDetails PutBadRequest(this HttpClient client, string link, object data)
        {
            var response = client.PutAsJsonAsync<object>(link, data).Result;
            response.EnsureBadRequestStatusCode();
            var problemDetails = response.Content.ReadAsAsync<SampleExam.Infrastructure.Errors.ApiProblemDetails>().Result;
            return problemDetails;
        }
        public static ExamDTO PostExamSuccesfully(this HttpClient client, string link, object data)
        {
            var response = client.PostAsJsonAsync<object>(link, data).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<ExamDTOEnvelope>().Result;
            var responseExam = envelope.Exam;
            return responseExam;
        }
        public static UserDTO PostUserSuccesfully(this HttpClient client, string link, object data)
        {
            var response = client.PostAsJsonAsync<object>(link, data).Result;
            response.EnsureSuccessStatusCode();
            var envelope = response.Content.ReadAsAsync<UserDTOEnvelope>().Result;
            var responseUser = envelope.User;
            return responseUser;
        }

        public static void GetNotFound(this HttpClient client, string link)
        {
            var response = client.GetAsync(link).Result;
            response.EnsureNotFoundStatusCode();
        }

        public static void DeleteNotFound(this HttpClient client, string link)
        {
            var response = client.DeleteAsync(link).Result;
            response.EnsureNotFoundStatusCode();
        }
        public static void PutNotFound(this HttpClient client, string link, object data)
        {
            var response = client.PutAsJsonAsync<object>(link, data).Result;
            response.EnsureNotFoundStatusCode();
        }
    }
}
using System.Net;
using System.Net.Http;

namespace SampleExamIntegrationTests.Helpers
{
    public static class HttpResponseMessageExtentions
    {

        public static void EnsureNotFoundStatusCode(this HttpResponseMessage message)
        {
            EnsureStatusCode(message, HttpStatusCode.NotFound);
        }

        public static void EnsureBadRequestStatusCode(this HttpResponseMessage message)
        {
            EnsureStatusCode(message, HttpStatusCode.BadRequest);
        }

        public static void EnsureUnauthorizedStatusCode(this HttpResponseMessage message)
        {
            EnsureStatusCode(message, HttpStatusCode.Unauthorized);
        }

        private static void EnsureStatusCode(HttpResponseMessage message, HttpStatusCode statusCode)
        {
            if (message.StatusCode != statusCode)
            {
                throw new HttpRequestException();
            }
        }
    }
}
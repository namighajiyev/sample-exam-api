using System.Net;
using System.Net.Http;

namespace SampleExamIntegrationTests.Helpers
{
    public static class HttpResponseMessageExtentions
    {
        public static void EnsureBadRequestStatusCode(this HttpResponseMessage message)
        {
            if (message.StatusCode != HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException();
            }
        }
    }
}
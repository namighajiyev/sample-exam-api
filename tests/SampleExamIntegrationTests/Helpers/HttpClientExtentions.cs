using System.Net.Http;

namespace SampleExamIntegrationTests.Helpers
{
    public static class HttpClientExtentions
    {
        private const string AUTH_HEADER_KEY = "Authorization";
        private const string AUTH_SCHEMA = "Bearer";
        public static void Authorize(this HttpClient client, string token)
        {
            if (client.DefaultRequestHeaders.Contains(AUTH_HEADER_KEY))
            {
                Unauthorize(client);
            }
            client.DefaultRequestHeaders.Add(AUTH_HEADER_KEY, $"{AUTH_SCHEMA} {token}");
        }

        public static void Unauthorize(this HttpClient client)
        {
            if (client.DefaultRequestHeaders.Contains(AUTH_HEADER_KEY))
            {
                client.DefaultRequestHeaders.Remove(AUTH_HEADER_KEY);
            }
        }
    }
}
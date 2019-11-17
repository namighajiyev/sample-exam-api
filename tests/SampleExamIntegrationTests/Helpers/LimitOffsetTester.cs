using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SampleExam.Features.Exam;
using Xunit;

namespace SampleExamIntegrationTests.Helpers
{
    public class LimitOffsetTester
    {
        private readonly HttpClient client;
        private readonly string getLink;

        public LimitOffsetTester(HttpClient client, string getLink)
        {
            this.client = client;
            this.getLink = getLink;
        }

        public int Limit { get; set; } = 2;
        public int Offest { get; set; } = 0;
        public Func<int, int, string> QueryStringFunc = (int limit, int offset) => $"limit={limit}&offset={offset}";

        public async Task DoTest<T>(Func<HttpClient, string, Task<Tuple<IEnumerable<T>, int>>> getCallFunc)
        {

            var count = 0;
            var offset = this.Offest;
            var haveExams = false;
            do
            {
                var link = $"{getLink}?{QueryStringFunc(Limit, offset)}";
                var tuple = await getCallFunc(client, link);
                var responseExams = tuple.Item1;
                var responseCount = responseExams.Count();
                count = tuple.Item2;
                offset += Limit;
                haveExams = offset < count;
                Assert.True(responseCount == Limit || (responseCount < Limit && !haveExams));
            }
            while (haveExams);
        }

    }
}
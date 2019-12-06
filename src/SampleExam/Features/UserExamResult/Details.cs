using MediatR;

namespace SampleExam.Features.UserExamResult
{
    public class Details
    {
        internal class Query : IRequest<UserExamResultDTOEnvelope>
        {
            private int userExamId;

            public Query(int userExamId)
            {
                this.userExamId = userExamId;
            }
        }
    }
}
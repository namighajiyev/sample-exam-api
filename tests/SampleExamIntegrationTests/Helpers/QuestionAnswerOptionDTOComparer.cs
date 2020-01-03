using System.Collections.Generic;
using SampleExam.Features.QuestionAnswer;

namespace SampleExamIntegrationTests.Helpers
{
    //
    public class QuestionAnswerOptionDTOComparer : IEqualityComparer<QuestionAnswerOptionDTO>
    {
        public bool Equals(QuestionAnswerOptionDTO x, QuestionAnswerOptionDTO y)
        {
            return x.AnswerOptionId == y.AnswerOptionId
            && x.CreatedAt == y.CreatedAt
            && x.QuestionId == y.QuestionId
            && x.UpdatedAt == y.UpdatedAt
            && x.UserExamId == y.UserExamId;
        }

        public int GetHashCode(QuestionAnswerOptionDTO obj)
        {
            return new
            {
                obj.AnswerOptionId,
                obj.CreatedAt,
                obj.QuestionId,
                obj.UpdatedAt,
                obj.UserExamId
            }.GetHashCode();
        }
    }
}

using System.Collections.Generic;
using SampleExam.Features.Answer;

namespace SampleExamIntegrationTests.Helpers
{
    public class AnswerOptionDtoComparer : IEqualityComparer<AnswerOptionDTO>
    {
        public bool Equals(AnswerOptionDTO x, AnswerOptionDTO y)
        {
            return x.CreatedAt == y.CreatedAt
            && x.Id == y.Id
            && x.IsRight == y.IsRight
            && x.QuestionId == y.QuestionId
            && x.Text == y.Text
            && x.UpdatedAt == y.UpdatedAt;
        }

        public int GetHashCode(AnswerOptionDTO obj)
        {
            return new { obj.CreatedAt, obj.Id, obj.IsRight, obj.QuestionId, obj.Text, obj.UpdatedAt }.GetHashCode();
        }
    }
}
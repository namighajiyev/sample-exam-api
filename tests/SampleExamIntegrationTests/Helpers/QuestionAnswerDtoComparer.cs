using System.Collections.Generic;
using System.Linq;
using SampleExam.Features.QuestionAnswer;

namespace SampleExamIntegrationTests.Helpers
{
    public class QuestionAnswerDtoComparer : IEqualityComparer<QuestionAnswerDTO>
    {
        public bool Equals(QuestionAnswerDTO x, QuestionAnswerDTO y)
        {
            var equal =
            x.CreatedAt.Equals(y.CreatedAt)
            && x.QuestionId == y.QuestionId
            && x.UpdatedAt.Equals(y.UpdatedAt)
            && x.UserExamId == y.UserExamId;

            var xAnswers = x.AnswerOptions.OrderBy(e => e.AnswerOptionId).ToArray();
            var yAnswers = y.AnswerOptions.OrderBy(e => e.AnswerOptionId).ToArray();
            var answersEqual = xAnswers.SequenceEqual(yAnswers, new QuestionAnswerOptionDTOComparer());
            return equal && answersEqual;
        }

        public int GetHashCode(QuestionAnswerDTO obj)
        {
            return new
            {
                obj.AnswerOptions,
                obj.CreatedAt,
                obj.QuestionId,
                obj.UpdatedAt,
                obj.UserExamId
            }.GetHashCode();
        }

    }
}
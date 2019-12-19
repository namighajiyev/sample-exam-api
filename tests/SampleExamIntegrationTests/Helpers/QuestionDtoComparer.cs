using System.Collections.Generic;
using System.Linq;
using SampleExam.Features.Question;

namespace SampleExamIntegrationTests.Helpers
{
    public class QuestionDtoComparer : IEqualityComparer<QuestionDTO>
    {
        public bool Equals(QuestionDTO x, QuestionDTO y)
        {
            var equal =
               x.Text == y.Text
            && x.Id == y.Id
            && x.AnswerOptions.Count == y.AnswerOptions.Count
            && x.CreatedAt.Equals(y.CreatedAt)
            && x.ExamId == y.ExamId
            && x.QuestionTypeId == y.QuestionTypeId
            && x.UpdatedAt == y.UpdatedAt;

            var xAnswers = x.AnswerOptions.OrderBy(e => e.Id).ToArray();
            var yAnswers = y.AnswerOptions.OrderBy(e => e.Id).ToArray();
            var answersEqual = xAnswers.SequenceEqual(yAnswers, new AnswerOptionDtoComparer());
            return equal && answersEqual;

        }

        public int GetHashCode(QuestionDTO obj)
        {
            return new
            {
                obj.AnswerOptions,
                obj.CreatedAt,
                obj.ExamId,
                obj.Id,
                obj.QuestionTypeId,
                obj.Text,
                obj.UpdatedAt
            }.GetHashCode();
        }
    }
}
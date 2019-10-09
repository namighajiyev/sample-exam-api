using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SampleExam.Features.Exam
{
    public static class ExamDbSetExtentions
    {
        public static IQueryable<Domain.Exam> ByIdAndUserId(
            this IQueryable<Domain.Exam> exams,
            int examId,
            int userId)
        {
            return exams.Where(e => e.Id == examId && e.UserId == userId);
        }

        public static IQueryable<Domain.Exam> NotPublishedByIdAndUserId(
            this IQueryable<Domain.Exam> exams,
            int examId,
            int userId)
        {
            return exams.Where(e => e.Id == examId && e.UserId == userId && !e.IsPublished);
        }

        public static IQueryable<Domain.Exam> PublishedAndNotPrivate(this IQueryable<Domain.Exam> exams)
        {
            return exams.Where(e => e.IsPublished && !e.IsPrivate);
        }

        public static IQueryable<Domain.Exam> IncludeTags(this IQueryable<Domain.Exam> exams)
        {
            return exams.Include(e => e.ExamTags).ThenInclude(e => e.Tag);
        }
    }
}
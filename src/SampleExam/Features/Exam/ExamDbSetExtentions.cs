using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SampleExam.Features.Exam
{
    public static class ExamDbSetExtentions
    {
        public static IQueryable<Domain.Exam> ByIdAndUserId(
            this DbSet<Domain.Exam> exams,
            int examId,
            int userId)
        {
            return exams.Where(e => e.Id == examId && e.UserId == userId);
        }

        public static IQueryable<Domain.Exam> NotPublishedByIdAndUserId(
            this DbSet<Domain.Exam> exams,
            int examId,
            int userId)
        {
            return exams.Where(e => e.Id == examId && e.UserId == userId && !e.IsPublished);
        }
    }
}
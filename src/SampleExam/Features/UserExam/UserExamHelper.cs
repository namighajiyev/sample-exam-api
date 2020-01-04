using System;
using System.Threading;
using System.Threading.Tasks;
using SampleExam.Infrastructure.Data;

namespace SampleExam.Features.UserExam
{
    public class UserExamHelper
    {
        public static async Task<bool> EndUserExamIfTimeExpired(
             SampleExamContext context,
             CancellationToken cancellationToken,
             Domain.UserExam userExam)
        {
            var expired = userExam.StartedtedAt.AddMinutes(userExam.Exam.TimeInMinutes) <= DateTime.UtcNow;
            if (expired)
            {
                userExam.EndedAt = userExam.StartedtedAt.AddMinutes(userExam.Exam.TimeInMinutes);
                await context.SaveChangesAsync(cancellationToken);
            }
            return expired;
        }
    }
}
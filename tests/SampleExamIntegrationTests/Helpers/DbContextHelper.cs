using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SampleExamIntegrationTests.Helpers
{
    public class DbContextHelper
    {
        private readonly DbContextFactory dbContextFactory;

        public DbContextHelper(DbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public SampleExam.Domain.Exam SelectExamWitTags(int examId)
        {
            SampleExam.Domain.Exam exam = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                exam = dbContext.Exams.Where(e => e.Id == examId).Include(e => e.ExamTags).First();
            }
            return exam;
        }

        public async Task<SampleExam.Domain.Exam> FindExamAsync(int examId)
        {
            SampleExam.Domain.Exam exam = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                exam = await dbContext.Exams.FindAsync(examId);
            }
            return exam;
        }

        public SampleExam.Domain.Exam SelectExamFirstOrDefault(int examId)
        {
            SampleExam.Domain.Exam exam = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                exam = dbContext.Exams.Where(e => e.Id == examId).FirstOrDefault();
            }
            return exam;
        }

        public SampleExam.Domain.Exam SelectExamIgnoreQueryFiltersTakeFirst(int examId)
        {
            SampleExam.Domain.Exam exam = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                exam = dbContext.Exams.Where(e => e.Id == examId).IgnoreQueryFilters().First();
            }
            return exam;
        }

        public async Task<SampleExam.Domain.Exam> PublishExamAsync(int examId)
        {
            SampleExam.Domain.Exam exam = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                exam = await dbContext.Exams.FindAsync(examId);
                exam.IsPublished = true;
                await dbContext.SaveChangesAsync();
            }
            return exam;
        }

        public int SelectQuestionCount(int questionId)
        {
            int count = 0;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                count = dbContext.Questions.Where(e => e.Id == questionId).Count();
            }
            return count;
        }



        public int SelectAnswerOptionCount(int answerOptionId)
        {
            int count = 0;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                count = dbContext.AnswerOptions.Where(e => e.Id == answerOptionId).Count();
            }
            return count;
        }


        public SampleExam.Domain.User SelectUserByEmail(string email)
        {
            SampleExam.Domain.User user = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                user = dbContext.Users.Where(e => e.Email == email).First();
            }
            return user;
        }

        public async Task<SampleExam.Domain.UserExam> UpdateUserExamStartDate(int userExamId, int minutesToAdd)
        {
            SampleExam.Domain.UserExam userExam = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                userExam = await dbContext.UserExams.FindAsync(userExamId);
                userExam.StartedtedAt = userExam.StartedtedAt.AddMinutes(minutesToAdd);
                await dbContext.SaveChangesAsync();
            }
            return userExam;
        }


        public async Task<SampleExam.Domain.UserExam> SelectUserExamAsync(int userExamId)
        {
            SampleExam.Domain.UserExam userExam = null;
            using (var dbContext = this.dbContextFactory.CreateDbContext())
            {
                userExam = await dbContext.UserExams.FindAsync(userExamId);
            }
            return userExam;
        }


    }
}
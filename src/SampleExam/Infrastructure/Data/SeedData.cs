using SampleExam.Domain;

namespace SampleExam.Infrastructure.Data
{
    public class SeedData
    {
        public class Genders
        {
            public static readonly Gender Male = new Gender() { Id = 1, Name = "Male" };
            public static readonly Gender Female = new Gender() { Id = 2, Name = "Female" };
        }

        public class QuestionTypes
        {
            public static readonly QuestionType Radio = new QuestionType() { Id = 1, Name = "Radio" };
            public static readonly QuestionType Checkbox = new QuestionType() { Id = 2, Name = "Checkbox" };
        }
    }
}
using System;
using System.Collections.Generic;

namespace SampleExam.Domain
{
    public class Exam
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TimeInMinutes { get; set; }

        public int PassPercentage { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsPublished { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public User User { get; set; }

        public ICollection<ExamTag> ExamTags { get; set; }
        public ICollection<UserExam> UserExams { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
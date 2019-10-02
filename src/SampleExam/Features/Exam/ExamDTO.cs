using System;
using System.Collections.Generic;

namespace SampleExam.Features.Exam
{

    public class ExamDTO
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

        public User.UserDTO User { get; set; }

        public ICollection<Tag.TagDTO> Tags { get; set; }

    }
}
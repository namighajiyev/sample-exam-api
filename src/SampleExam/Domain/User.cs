using System;
using System.Collections.Generic;

namespace SampleExam.Domain
{
    public class User
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Middlename { get; set; }

        public int GenderId { get; set; }

        public DateTime Dob { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Gender Gender { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
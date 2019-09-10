using System;

namespace SampleExam.Features.User
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Middlename { get; set; }

        public int GenderId { get; set; }

        public DateTime Dob { get; set; }

        public string Email { get; set; }

    }
}
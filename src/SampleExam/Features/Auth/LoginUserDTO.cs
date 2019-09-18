using System;

namespace SampleExam.Features.Auth
{
    public class LoginUserDTO
    {

        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Middlename { get; set; }

        public int GenderId { get; set; }

        public DateTime Dob { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string RefresToken { get; set; }


    }
}
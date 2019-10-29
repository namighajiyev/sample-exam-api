using System;
using SampleExam.Infrastructure.Utils;

namespace SampleExam.Infrastructure.Security
{
    public class ApiTokenRefreshTokenGenrator : IApiTokenRefreshTokenGenrator
    {
        public string GenerateToken()
        {
            return Guid.NewGuid().ToGuidString();
        }
    }
}
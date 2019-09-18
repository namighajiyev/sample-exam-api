using System;

namespace SampleExam.Infrastructure.Security
{
    public class ApiTokenRefreshTokenGenrator : IApiTokenRefreshTokenGenrator
    {
        public string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
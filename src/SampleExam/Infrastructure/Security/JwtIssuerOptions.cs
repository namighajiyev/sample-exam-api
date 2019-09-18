using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace SampleExam.Infrastructure.Security
{
    public class JwtIssuerOptions
    {
        //public const string Schemes = "Bearer"; //JwtBearerDefaults.AuthenticationScheme
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public string Audience { get; set; }
        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime IssuedAt => DateTime.UtcNow;
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(5);
        public DateTime Expiration => IssuedAt.Add(ValidFor);
        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
        public SigningCredentials SigningCredentials { get; set; }
    }
}
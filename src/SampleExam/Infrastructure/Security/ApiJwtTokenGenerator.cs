using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace SampleExam.Infrastructure.Security
{

    public class ApiJwtTokenGenerator : IApiJwtTokenGenerator
    {
        private readonly JwtIssuerOptions jwtOptions;

        public ApiJwtTokenGenerator(IOptions<JwtIssuerOptions> jwtOptions) => this.jwtOptions = jwtOptions.Value;
        public async Task<string> GenerateToken(int userId, string email, uint? expireMinutes = null)
        {
            var header = new JwtHeader(jwtOptions.SigningCredentials);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, await jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,new DateTimeOffset(jwtOptions.IssuedAt)
                .ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Iss, jwtOptions.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, jwtOptions.Audience),
                new Claim(JwtRegisteredClaimNames.Nbf,new DateTimeOffset(jwtOptions.NotBefore)
                .ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp,
                (expireMinutes.HasValue ? new DateTimeOffset(jwtOptions.IssuedAt.AddMinutes((double)expireMinutes)) :
                new DateTimeOffset(jwtOptions.Expiration))
                .ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };
            var payload = new JwtPayload(claims);
            var jwt = new JwtSecurityToken(header, payload);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}

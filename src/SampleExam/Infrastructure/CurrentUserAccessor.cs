using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SampleExam.Infrastructure
{
    public class CurrentUser : ICurrentUser
    {

        public CurrentUser(int userId, string email)
        {
            this.UserId = userId;
            this.Email = email;
        }
        public int UserId { get; set; }
        public string Email { get; set; }
    }
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public ICurrentUser GetCurrentUser()
        {
            var id = GetCurrentUserId();
            var email = GetCurrentUserEmail();
            return new CurrentUser(id, email);
        }

        public int GetCurrentUserId()
        {
            var subject = httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(subject);
        }

        public string GetCurrentUserEmail()
        {
            var email = httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            return email;
        }
    }
}
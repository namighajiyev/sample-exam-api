using System.Threading.Tasks;

namespace SampleExam.Infrastructure.Security
{
    public interface IApiJwtTokenGenerator
    {
        Task<string> GenerateToken(int userId, string email, uint? expireMinutes = null);
    }
}
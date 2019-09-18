namespace SampleExam.Infrastructure.Security
{
    public interface IApiTokenRefreshTokenGenrator
    {
        string GenerateToken();
    }
}
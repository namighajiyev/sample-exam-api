namespace SampleExam.Features.Auth
{
    public class LoginUserDTOEnvelope
    {
        public LoginUserDTOEnvelope(LoginUserDTO user)
        {
            this.User = user;
        }
        public LoginUserDTO User { get; set; }
    }
}
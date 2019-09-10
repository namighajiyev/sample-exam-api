namespace SampleExam.Features.User
{
    public class UserDTOEnvelope
    {
        public UserDTOEnvelope(UserDTO user)
        {
            this.User = user;
        }
        public UserDTO User { get; set; }
    }
}
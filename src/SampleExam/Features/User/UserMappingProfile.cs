using AutoMapper;

namespace SampleExam.Features.User
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<Domain.User, UserDTO>(MemberList.Destination);
            // .ForMember(e => e.Token, (config) => { config.Ignore});
            CreateMap<Create.UserData, Domain.User>(MemberList.None);

        }
    }
}
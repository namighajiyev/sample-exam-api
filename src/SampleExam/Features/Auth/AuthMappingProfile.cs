using AutoMapper;
using SampleExam.Features.Auth;

namespace SampleExam.Features.Auth
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<Domain.User, LoginUserDTO>(MemberList.Destination);
            // .ForMember(e => e.Token, (config) => { config.Ignore});
        }
    }
}
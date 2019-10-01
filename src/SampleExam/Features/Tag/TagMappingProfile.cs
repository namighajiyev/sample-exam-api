using AutoMapper;

namespace SampleExam.Features.Tag
{
    public class TagMappingProfile : Profile
    {
        public TagMappingProfile()
        {
            CreateMap<Domain.Tag, TagDTO>(MemberList.Destination);
            //CreateMap<Create.UserData, Domain.User>(MemberList.None);

        }
    }
}
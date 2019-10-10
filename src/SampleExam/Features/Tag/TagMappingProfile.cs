using AutoMapper;

namespace SampleExam.Features.Tag
{
    public class TagMappingProfile : Profile
    {
        public TagMappingProfile()
        {
            CreateMap<Domain.Tag, TagDTO>(MemberList.Destination)
            .ForMember(dto => dto.Tag, (config) => config.MapFrom(t => t.TagId));
        }
    }
}
using AutoMapper;

namespace SampleExam.Features.Values
{
    public class ValueProfile : Profile
    {
        public ValueProfile()
        {
            CreateMap<Domain.Value, ValueDTO>(MemberList.Destination);
        }
    }
}
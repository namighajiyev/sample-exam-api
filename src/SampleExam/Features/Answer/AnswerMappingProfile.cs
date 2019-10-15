using AutoMapper;
namespace SampleExam.Features.Answer
{
    public class AnswerMappingProfile : Profile
    {
        public AnswerMappingProfile()
        {
            CreateMap<Domain.AnswerOption, AnswerOptionDTO>(MemberList.Destination);
        }
    }
}
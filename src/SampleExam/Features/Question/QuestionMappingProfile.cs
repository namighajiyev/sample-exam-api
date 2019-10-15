using AutoMapper;
namespace SampleExam.Features.Question
{
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {
            CreateMap<Domain.Question, QuestionDTO>(MemberList.Destination);
        }

    }

}

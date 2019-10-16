using AutoMapper;
namespace SampleExam.Features.Question
{
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {
            CreateMap<Domain.Question, QuestionDTO>(MemberList.Destination);
            CreateMap<QuestionDTO, Domain.Question>(MemberList.Destination);
            CreateMap<Create.QuestionData, Domain.Question>(MemberList.Destination)
            .ForMember(e => e.AnswerOptions, options => options.MapFrom(e => e.Answers));
            CreateMap<Create.AnswerData, Domain.AnswerOption>(MemberList.Destination);
            CreateMap<Edit.QuestionData, Domain.Question>(MemberList.Destination)
            .ForMember(e => e.AnswerOptions, options => options.MapFrom(e => e.Answers));
            CreateMap<Edit.AnswerData, Domain.AnswerOption>(MemberList.Destination);
        }

    }

}

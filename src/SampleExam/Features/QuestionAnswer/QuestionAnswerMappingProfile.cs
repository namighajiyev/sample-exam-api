using AutoMapper;

namespace SampleExam.Features.QuestionAnswer
{
    public class QuestionAnswerMappingProfile : Profile
    {
        public QuestionAnswerMappingProfile()
        {
            CreateMap<Domain.UserExamQuestionAnswr, QuestionAnswerOptionDTO>(MemberList.Destination);
            CreateMap<Domain.UserExamQuestion, QuestionAnswerDTO>(MemberList.Destination)
               .ForMember(e => e.AnswerOptions, options => options.MapFrom(e => e.UserExamQuestionAnswers));
        }

    }
}
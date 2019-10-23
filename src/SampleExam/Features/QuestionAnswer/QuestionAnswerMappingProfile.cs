using AutoMapper;

namespace SampleExam.Features.QuestionAnswer
{
    public class QuestionAnswerMappingProfile : Profile
    {
        public QuestionAnswerMappingProfile()
        {
            CreateMap<Domain.UserExamQuestionAnswer, QuestionAnswerDTO>(MemberList.Destination);
        }

    }
}
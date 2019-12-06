using AutoMapper;

namespace SampleExam.Features.UserExamResult
{
    public class UserExamResultMappingProfile : Profile
    {
        public UserExamResultMappingProfile()
        {
            CreateMap<Domain.UserExamResult, UserExamResultDTO>(MemberList.Destination);
        }
    }
}
using AutoMapper;

namespace SampleExam.Features.UserExam
{
    public class UserExamMappingProfile : Profile
    {
        public UserExamMappingProfile()
        {
            CreateMap<Domain.UserExam, UserExamDTO>(MemberList.Destination);
        }

    }
}
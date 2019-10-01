using AutoMapper;

namespace SampleExam.Features.Exam
{
    public class ExamMappingProfile : Profile
    {
        public ExamMappingProfile()
        {
            CreateMap<Domain.Exam, ExamDTO>(MemberList.Destination);
        }
    }
}
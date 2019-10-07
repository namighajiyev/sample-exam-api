using AutoMapper;
using System.Linq;

namespace SampleExam.Features.Exam
{
    public class ExamMappingProfile : Profile
    {
        public ExamMappingProfile()
        {
            CreateMap<Domain.Exam, ExamDTO>(MemberList.Destination)
            .ForMember(e => e.Tags, options =>
             options.MapFrom(e => e.ExamTags.Select(ext => ext.Tag)));

            CreateMap<Create.ExamData, Domain.Exam>(MemberList.Destination);
        }
    }
}
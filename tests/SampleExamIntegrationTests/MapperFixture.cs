using AutoMapper;

namespace SampleExamIntegrationTests
{
    public class MapperFixture
    {
        private IMapper mapper;
        public IMapper Mapper
        {
            get
            {
                return mapper;
            }
        }
        public MapperFixture()
        {
            var configuration = new MapperConfiguration(cfg =>
               {
                   cfg.AddMaps(typeof(SampleExam.Startup).Assembly);
               });
            this.mapper = configuration.CreateMapper();
        }
    }
}
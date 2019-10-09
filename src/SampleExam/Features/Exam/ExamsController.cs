using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static SampleExam.Features.Exam.Enums;

namespace SampleExam.Features.Exam
{
    [Route("exams")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<ExamsDTOEnvelope> GetPublishedExams(
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new List.Query(null, limit, offset, includeTags, includeUser));
        }

        [HttpGet("user_exams/{userId}")]
        public async Task<ExamsDTOEnvelope> GetUserExams(
            int userId,
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new List.Query(userId, limit, offset, includeTags, includeUser));
        }

        [HttpGet("user_exams")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamsDTOEnvelope> GetCurrenUserExams(
            [FromQuery] PublishType publishType,
            [FromQuery] PrivateType privateType,
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new UserExamList.Query(
                publishType, privateType, limit, offset, includeTags, includeUser));
        }

    }
}
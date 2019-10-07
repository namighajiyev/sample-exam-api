using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SampleExam.Features.Exam
{
    [Route("exam")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExamController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<ExamsDTOEnvelope> Get(
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new List.Query(null, limit, offset, false, includeTags, includeUser));
        }


        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamsDTOEnvelope> GetCurrenUserExams(
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new List.Query(null, limit, offset, true, includeTags, includeUser));
        }

        [HttpGet("user/{userId}")]
        public async Task<ExamsDTOEnvelope> GetUserExams(
             int userId,
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new List.Query(userId, limit, offset, false, includeTags, includeUser));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamDTOEnvelope> Signup([FromBody] Create.Request command)
        {
            var result = await _mediator.Send(command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return result;
        }

    }
}
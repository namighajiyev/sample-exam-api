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

        [HttpGet("exam/{id}")]
        public async Task<ExamDTOEnvelope> GetPublishedExam(
            int id,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new Details.Query(id, includeTags, includeUser));
        }

        [HttpGet("user/exam/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamDTOEnvelope> GetCurrentUserExam(
            int id,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new UserExamDetail.Query(id, includeTags, includeUser));
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

        [HttpGet("exam/user/{userId}")]
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

        [HttpGet("user/exams")]
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



        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamDTOEnvelope> Create([FromBody] Create.Request command)
        {
            var result = await _mediator.Send(command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamDTOEnvelope> Delete(int id)
        {
            return await _mediator.Send(new Delete.Request(id));
        }


        [HttpPut("publish/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamDTOEnvelope> Publish(int id)
        {
            return await _mediator.Send(new Publish.Request(id));
        }


        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ExamDTOEnvelope> Edit(int id, [FromBody]Edit.Request request)
        {
            request.Exam.Id = id;
            return await _mediator.Send(request);
        }
    }
}
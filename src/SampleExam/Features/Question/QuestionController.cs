using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SampleExam.Features.Question
{
    [Route("questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {

        private readonly IMediator _mediator;

        public QuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<QuestionsDTOEnvelope> GetPublishedExams(
                  [FromQuery] int examId,
                  [FromQuery] int? limit,
                  [FromQuery] int? offset,
                  [FromQuery] bool? includeAnswerOptions
                  )
        {
            var query = new List.Query(examId, limit, offset, includeAnswerOptions);
            return await _mediator.Send(query);
        }


        [HttpPost("{examId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<QuestionDTOEnvelope> Create(int examId, [FromBody] Create.Request command)
        {
            var result = await _mediator.Send(command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return result;
        }

    }
}
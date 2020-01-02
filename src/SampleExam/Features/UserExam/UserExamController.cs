using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleExam.Features.UserExam
{
    [Route("userexams")]
    [ApiController]
    public class UserExamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserExamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserExamsDTOEnvelope> Get(
          [FromQuery] int? limit,
          [FromQuery] int? offset,
          [FromQuery] bool? includeExams
          )
        {
            var query = new List.Query(limit, offset, includeExams);
            return await _mediator.Send(query);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserExamDTOEnvelope> Get(int id, [FromQuery] bool? includeExams)
        {
            return await _mediator.Send(new Details.Query(id, includeExams));
        }


        [HttpPost("{examId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserExamDTOEnvelope> Create(int examId)
        {
            var command = new Create.Request(examId);
            var result = await _mediator.Send(command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserExamDTOEnvelope> Edit(int id)
        {
            return await _mediator.Send(new Edit.Request(id));
        }
    }
}
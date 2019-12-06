using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleExam.Features.UserExamResult
{
    [Route("userexamresults")]
    [ApiController]
    public class UserExamResultController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserExamResultController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{userExamId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserExamResultDTOEnvelope> GetPublishedExam(
            int userExamId
            )
        {
            return await _mediator.Send(new Details.Query(userExamId));
        }

        [HttpPost("{userExamId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<UserExamResultDTOEnvelope> Create(int userExamId)
        {
            var command = new Create.Request(userExamId);
            var result = await _mediator.Send(command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return result;
        }

    }
}
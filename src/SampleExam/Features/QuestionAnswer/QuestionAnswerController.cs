using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleExam.Features.QuestionAnswer
{
    [Route("questionanswers")]
    [ApiController]
    public class QuestionAnswerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionAnswerController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<QuestionAnswerDTOEnvelope> CreateOrUpdate([FromBody] CreateOrUpdate.Request command)
        {
            var result = await _mediator.Send(command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return result;
        }

    }
}
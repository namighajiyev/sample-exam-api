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
        public async Task<QuestionsDTOEnvelope> GetPublishedNotPrivateExamQuestions(
                  [FromQuery] int examId,
                  [FromQuery] int? limit,
                  [FromQuery] int? offset,
                  [FromQuery] bool? includeAnswerOptions
                  )
        {
            var query = new List.Query(false, examId, limit, offset, includeAnswerOptions);
            return await _mediator.Send(query);
        }

        [HttpGet("/user/exam/questions")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<QuestionsDTOEnvelope> GetUserExamQuestions(
                  [FromQuery] int examId,
                  [FromQuery] int? limit,
                  [FromQuery] int? offset,
                  [FromQuery] bool? includeAnswerOptions
                  )
        {
            var query = new List.Query(true, examId, limit, offset, includeAnswerOptions);
            return await _mediator.Send(query);
        }


        [HttpGet("{id}")]
        public async Task<QuestionDTOEnvelope> GetPublishedNotPrivateExamQuestion(
                  int id,
                  [FromQuery] bool? includeAnswerOptions
            )
        {
            return await _mediator.Send(new Details.Query(false, id, includeAnswerOptions));
        }

        [HttpGet("/user/exam/question/{id}")]
        public async Task<QuestionDTOEnvelope> GetUserExamQuestion(
                  int id,
                  [FromQuery] bool? includeAnswerOptions
            )
        {
            return await _mediator.Send(new Details.Query(true, id, includeAnswerOptions));
        }

        [HttpPost("{examId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<QuestionDTOEnvelope> Create(int examId, [FromBody] Create.Request command)
        {
            command.ExamId = examId;
            var result = await _mediator.Send(command);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<QuestionDTOEnvelope> Edit(int id, [FromBody]Edit.Request request)
        {
            request.Question.Id = id;
            return await _mediator.Send(request);
        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<QuestionDTOEnvelope> Delete(int id)
        {
            return await _mediator.Send(new Delete.Request(id));
        }

    }
}
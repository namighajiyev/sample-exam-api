using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
            [FromQuery] int? userId,
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] bool? isCurrentUser,
            [FromQuery] bool? includeTags,
            [FromQuery] bool? includeUser
            )
        {
            return await _mediator.Send(new List.Query(userId, limit, offset, isCurrentUser, includeTags, includeUser));
        }

    }
}
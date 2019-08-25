using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SampleExam.Features.Values
{
    [Route("values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ValuesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ValuesDTOEnvelop> Get([FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(limit, offset));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ValueDTOEnvelope> Get(int id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        // POST api/values
        [HttpPost]
        public async Task<ValueDTOEnvelope> Post([FromBody] Create.Request command)
        {
            return await _mediator.Send(command);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<ValueDTOEnvelope> Put(int id, [FromBody] Edit.Request value)
        {
            value.Value.Id = id;
            return await _mediator.Send(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _mediator.Send(new Delete.Request(id));
        }
    }
}

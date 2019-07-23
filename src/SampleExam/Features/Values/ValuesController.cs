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
        public async Task<ValuesEnvelop> Get([FromQuery] int? limit, [FromQuery] int? offset)
        {
            return await _mediator.Send(new List.Query(limit, offset));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ValueEnvelop> Get(int id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        // POST api/values
        [HttpPost]
        public async Task<ValueEnvelop> Post([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

using Application.MediatorHandler.MediatorCommend;
using Application.MediatorHandler.MediatorQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers.MessageController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromForm]SendMessageCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllMessagesForSender query)
        {
            var messages = await _mediator.Send(query);

            if (messages == null || !messages.Any())
                return NoContent();

            return Ok(messages);
        }


        [HttpPost("seen")]
        public async Task<IActionResult> Seen(SeenMessageCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}

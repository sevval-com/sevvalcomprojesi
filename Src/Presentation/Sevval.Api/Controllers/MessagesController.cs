using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sevval.Api.Dtos.Messaging;
using Sevval.Application.Features.Messaging.Commands.SendMessage;
using Sevval.Application.Features.Messaging.Queries.GetConversationMessages;

namespace Sevval.Api.Controllers
{
    [ApiController]
    [Route("api/v1/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SendMessageResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SendMessageResponseDto), StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDto request, CancellationToken cancellationToken)
        {
            var command = new SendMessageCommand
            {
                SenderId = request.SenderId,
                RecipientId = request.RecipientId,
                Body = request.Body
            };

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                var failureResponse = new SendMessageResponseDto
                {
                    IsSuccess = false,
                    Error = result.Error
                };

                return StatusCode(StatusCodes.Status429TooManyRequests, failureResponse);
            }

            var successResponse = new SendMessageResponseDto
            {
                MessageId = result.MessageId,
                CreatedOnUtc = result.CreatedOnUtc,
                Status = result.Status.ToString(),
                IsSuccess = true
            };

            return StatusCode(StatusCodes.Status201Created, successResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetConversationMessagesResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConversationMessages([FromQuery] GetConversationMessagesRequestDto request, CancellationToken cancellationToken)
        {
            var query = new GetConversationMessagesQuery
            {
                UserId = request.UserId,
                OtherUserId = request.OtherUserId,
                Page = request.Page,
                PageSize = request.PageSize
            };

            var result = await _mediator.Send(query, cancellationToken);

            var response = new GetConversationMessagesResponseDto
            {
                Items = result
                    .Select(m => new MessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        RecipientId = m.RecipientId,
                        Body = m.Body,
                        CreatedOnUtc = m.CreatedOnUtc,
                        Status = m.Status.ToString()
                    })
                    .ToList()
            };

            return Ok(response);
        }
    }
}

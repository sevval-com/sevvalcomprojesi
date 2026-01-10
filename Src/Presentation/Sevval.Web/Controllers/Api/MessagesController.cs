using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Messaging.Commands.MarkConversationAsRead;
using Sevval.Application.Features.Messaging.Commands.SendMessage;
using Sevval.Application.Features.Messaging.Commands.UndoConversationRead;
using Sevval.Application.Features.Messaging.Queries.GetConversationsForUser;
using Sevval.Application.Features.Messaging.Queries.GetConversationMessages;
using Sevval.Application.Features.Messaging.Queries.GetUnreadCountsByCategory;
using Sevval.Domain.Messaging;
using Sevval.Web.Dtos.Messaging;

namespace Sevval.Web.Controllers.Api;

[ApiController]
[Route("api/v1/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ping")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Ping() => Ok("pong");

    [HttpGet("unread-counts")]
    [ProducesResponseType(typeof(UnreadCountsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCounts(
        [FromQuery] UnreadCountsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new GetUnreadCountsByCategoryQuery
        {
            UserId = request.CurrentUserId
        };

        var result = await _mediator.Send(query, cancellationToken);

        var response = new UnreadCountsResponseDto
        {
            Counts = result.Counts.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value)
        };

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SendMessageResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SendMessageResponseDto), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new SendMessageCommand
        {
            SenderId = request.SenderId,
            RecipientId = request.RecipientId,
            Body = request.Body,
            MessageType = request.MessageType ?? MessageType.Other,
            ListingId = request.ListingId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            var failureResponse = new SendMessageResponseDto
            {
                IsSuccess = false,
                Error = result.Error
            };

            return result.Error switch
            {
                "ListingNotFound" => StatusCode(StatusCodes.Status404NotFound, failureResponse),
                "ListingOwnerNotFound" => StatusCode(StatusCodes.Status400BadRequest, failureResponse),
                "ListingOwnerMismatch" => StatusCode(StatusCodes.Status403Forbidden, failureResponse),
                _ => StatusCode(StatusCodes.Status429TooManyRequests, failureResponse)
            };
        }

        var successResponse = new SendMessageResponseDto
        {
            MessageId = result.MessageId ?? Guid.Empty,
            CreatedOnUtc = result.CreatedOnUtc ?? DateTime.UtcNow,
            Status = result.Status.ToString(),
            IsSuccess = true
        };

        return StatusCode(StatusCodes.Status201Created, successResponse);
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetConversationMessagesResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversationMessages(
        [FromQuery] GetConversationMessagesRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new GetConversationMessagesQuery
        {
            UserId = request.UserId,
            OtherUserId = request.OtherUserId,
            Page = request.Page,
            PageSize = request.PageSize,
            MessageType = request.MessageType,
            ListingId = request.ListingId
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
                    Status = m.IsRead ? "Read" : m.Status.ToString(),
                    MessageType = m.MessageType.ToString()
                })
                .ToList()
        };

        return Ok(response);
    }

    [HttpGet("conversations")]
    [ProducesResponseType(typeof(GetConversationsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations(
        [FromQuery] GetConversationsRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new GetConversationsForUserQuery
        {
            UserId = request.UserId,
            MessageType = request.MessageType
        };

        var result = await _mediator.Send(query, cancellationToken);

        var response = new GetConversationsResponseDto
        {
            Items = result.Select(item => new ConversationSummaryDto
            {
                OtherUserId = item.OtherUserId,
                OtherUserFullName = item.OtherUserFullName,
                OtherUserEmail = item.OtherUserEmail,
                OtherUserAvatarUrl = item.OtherUserAvatarUrl,
                LastMessagePreview = item.LastMessagePreview,
                LastMessageAtUtc = item.LastMessageAtUtc,
                UnreadCount = item.UnreadCount,
                MessageType = item.MessageType.ToString(),
                ListingId = item.ListingId
            }).ToList()
        };

        return Ok(response);
    }

    [HttpPost("mark-read")]
    [ProducesResponseType(typeof(MarkConversationAsReadResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkConversationAsRead(
        [FromBody] MarkConversationAsReadRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new MarkConversationAsReadCommand
        {
            ReaderId = request.ReaderId,
            OtherUserId = request.OtherUserId,
            ListingId = request.ListingId
        };

        var result = await _mediator.Send(command, cancellationToken);

        var response = new MarkConversationAsReadResponseDto
        {
            MessageIds = result.MessageIds,
            ReadOnUtc = result.ReadOnUtc
        };

        return Ok(response);
    }

    [HttpPost("undo-read")]
    [ProducesResponseType(typeof(UndoConversationReadResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UndoConversationRead(
        [FromBody] UndoConversationReadRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new UndoConversationReadCommand
        {
            ReaderId = request.ReaderId,
            OtherUserId = request.OtherUserId,
            MessageIds = request.MessageIds,
            ListingId = request.ListingId
        };

        var result = await _mediator.Send(command, cancellationToken);

        var response = new UndoConversationReadResponseDto
        {
            MessageIds = result.MessageIds,
            DeliveredOnUtc = result.DeliveredOnUtc
        };

        return Ok(response);
    }
}

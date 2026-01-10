using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sevval.Api.Controllers;
using Sevval.Api.Dtos.Messaging;
using Sevval.Application.Features.Messaging.Commands.SendMessage;
using Xunit;

namespace Sevval.Messaging.Tests;

public class MessagesControllerListingIdTests
{
    [Theory]
    [InlineData("ListingNotFound", 404)]
    [InlineData("ListingOwnerNotFound", 400)]
    [InlineData("ListingOwnerMismatch", 403)]
    public async Task SendMessage_MapsErrorsToStatusCodes(string error, int statusCode)
    {
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<SendMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResult
            {
                IsSuccess = false,
                Error = error
            });

        var controller = new MessagesController(mediator.Object);
        var request = new SendMessageRequestDto
        {
            SenderId = "sender-id",
            RecipientId = "recipient-id",
            Body = "Test",
            ListingId = 123
        };

        var result = await controller.SendMessage(request, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(statusCode, objectResult.StatusCode);
    }
}

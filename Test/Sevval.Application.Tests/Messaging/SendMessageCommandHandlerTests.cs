using Moq;
using Sevval.Application.Features.Messaging.Commands.SendMessage;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Domain.Messaging;
using MessagingMessage = Sevval.Domain.Messaging.Message;
using Xunit;

namespace Sevval.Application.Tests.Messaging;

public class SendMessageCommandHandlerTests
{
    private readonly Mock<IMessageReadRepository> _readRepositoryMock = new();
    private readonly Mock<IMessageWriteRepository> _writeRepositoryMock = new();
    private readonly Mock<IDateTimeService> _dateTimeServiceMock = new();

    private SendMessageCommandHandler CreateHandler() =>
        new(_writeRepositoryMock.Object, _readRepositoryMock.Object, _dateTimeServiceMock.Object);

    [Fact]
    public async Task Handle_UnderLimit_PersistsMessageAndReturnsSuccess()
    {
        // İşlem
        var now = new DateTime(2024, 12, 25, 10, 30, 0, DateTimeKind.Utc);
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(now);
        _readRepositoryMock
            .Setup(x => x.CountSentAsync("sender-1", now.Date, now.Date.AddDays(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(999);

        var command = new SendMessageCommand
        {
            SenderId = "sender-1",
            RecipientId = "recipient-1",
            Body = "hello"
        };

        var handler = CreateHandler();

        // Çalıştırma
        var result = await handler.Handle(command, CancellationToken.None);

        // Beklenen sonuç
        Assert.True(result.IsSuccess);
        Assert.Equal(MessageStatus.Delivered, result.Status);
        Assert.Equal(now, result.CreatedOnUtc);
        _readRepositoryMock.Verify(x => x.CountSentAsync("sender-1", now.Date, now.Date.AddDays(1), It.IsAny<CancellationToken>()), Times.Once);
        _writeRepositoryMock.Verify(x =>
            x.AddAsync(
                It.Is<MessagingMessage>(m =>
                    m.SenderId == "sender-1" &&
                    m.RecipientId == "recipient-1" &&
                    m.Body == "hello" &&
                    m.CreatedOnUtc == now),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AtLimit_DoesNotPersistAndReturnsFailure()
    {
        // Hazırlık
        var now = new DateTime(2024, 12, 25, 10, 30, 0, DateTimeKind.Utc);
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(now);
        _readRepositoryMock
            .Setup(x => x.CountSentAsync("sender-1", now.Date, now.Date.AddDays(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000);

        var command = new SendMessageCommand
        {
            SenderId = "sender-1",
            RecipientId = "recipient-1",
            Body = "hello"
        };

        var handler = CreateHandler();

        // Çalıştırma
        var result = await handler.Handle(command, CancellationToken.None);

        // Doğrulama
        Assert.False(result.IsSuccess);
        Assert.Equal("Daily message limit exceeded.", result.Error);
        _readRepositoryMock.Verify(x => x.CountSentAsync("sender-1", now.Date, now.Date.AddDays(1), It.IsAny<CancellationToken>()), Times.Once);
        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MessagingMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_OverLimit_DoesNotPersistAndReturnsFailure()
    {
        // Hazırlık
        var now = new DateTime(2024, 12, 25, 10, 30, 0, DateTimeKind.Utc);
        _dateTimeServiceMock.Setup(x => x.UtcNow).Returns(now);
        _readRepositoryMock
            .Setup(x => x.CountSentAsync("sender-1", now.Date, now.Date.AddDays(1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1500);

        var command = new SendMessageCommand
        {
            SenderId = "sender-1",
            RecipientId = "recipient-1",
            Body = "hello"
        };

        var handler = CreateHandler();

        // Çalıştırma
        var result = await handler.Handle(command, CancellationToken.None);

        // Doğrulama
        Assert.False(result.IsSuccess);
        Assert.Equal("Daily message limit exceeded.", result.Error);
        _readRepositoryMock.Verify(x => x.CountSentAsync("sender-1", now.Date, now.Date.AddDays(1), It.IsAny<CancellationToken>()), Times.Once);
        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MessagingMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

using Sevval.Messaging.Tests.Fixtures;
using Sevval.Messaging.Tests.Helpers;
using Sevval.Persistence.Repositories.Messaging;
using Sevval.Domain.Messaging;
using Xunit;

namespace Sevval.Messaging.Tests;

public class MessageRepositoryListingIdTests : IClassFixture<SqliteInMemoryDbFixture>
{
    private readonly SqliteInMemoryDbFixture _fixture;

    public MessageRepositoryListingIdTests(SqliteInMemoryDbFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task GetConversationAsync_ListingIdNull_ReturnsOnlyNullListingMessages()
    {
        await using var context = _fixture.CreateContext();
        var repository = new MessageRepository(context);

        var senderId = "sender-1";
        var recipientId = "recipient-1";

        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, DateTime.UtcNow.AddMinutes(-5), null));
        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, DateTime.UtcNow.AddMinutes(-4), 123));
        await context.SaveChangesAsync();

        var result = await repository.GetConversationAsync(senderId, recipientId, 1, 20, null, null, CancellationToken.None);

        Assert.NotEmpty(result);
        Assert.All(result, message => Assert.Null(message.ListingId));
    }

    [Fact]
    public async Task GetConversationAsync_WithListingId_ReturnsOnlyMatchingListingMessages()
    {
        await using var context = _fixture.CreateContext();
        var repository = new MessageRepository(context);

        var senderId = "sender-2";
        var recipientId = "recipient-2";

        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, DateTime.UtcNow.AddMinutes(-5), 123));
        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, DateTime.UtcNow.AddMinutes(-4), 456));
        await context.SaveChangesAsync();

        var result = await repository.GetConversationAsync(senderId, recipientId, 1, 20, null, 123, CancellationToken.None);

        Assert.NotEmpty(result);
        Assert.All(result, message => Assert.Equal(123, message.ListingId));
    }

    [Fact]
    public async Task GetConversationAsync_SameUsers_DifferentListingIds_DoNotMixThreads()
    {
        await using var context = _fixture.CreateContext();
        var repository = new MessageRepository(context);

        var senderId = "sender-3";
        var recipientId = "recipient-3";

        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, DateTime.UtcNow.AddMinutes(-5), null));
        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, DateTime.UtcNow.AddMinutes(-4), 123));
        await context.SaveChangesAsync();

        var nullListing = await repository.GetConversationAsync(senderId, recipientId, 1, 20, null, null, CancellationToken.None);
        var listing123 = await repository.GetConversationAsync(senderId, recipientId, 1, 20, null, 123, CancellationToken.None);

        Assert.All(nullListing, message => Assert.Null(message.ListingId));
        Assert.All(listing123, message => Assert.Equal(123, message.ListingId));
    }

    [Fact]
    public async Task GetConversationAsync_PaginationAndOrder_NewestFirst()
    {
        await using var context = _fixture.CreateContext();
        var repository = new MessageRepository(context);

        var senderId = "sender-4";
        var recipientId = "recipient-4";
        var baseTime = DateTime.UtcNow.AddMinutes(-10);

        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, baseTime.AddMinutes(1), 123));
        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, baseTime.AddMinutes(2), 123));
        await context.Set<Message>().AddAsync(TestDataFactory.CreateMessage(Guid.NewGuid(), senderId, recipientId, baseTime.AddMinutes(3), 123));
        await context.SaveChangesAsync();

        var result = await repository.GetConversationAsync(senderId, recipientId, 1, 2, null, 123, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.True(result[0].CreatedOnUtc >= result[1].CreatedOnUtc);
    }
}

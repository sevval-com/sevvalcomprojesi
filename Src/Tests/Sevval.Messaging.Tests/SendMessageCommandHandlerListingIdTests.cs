using Sevval.Application.Features.Messaging.Commands.SendMessage;
using Sevval.Messaging.Tests.Fixtures;
using Sevval.Messaging.Tests.Helpers;
using Sevval.Persistence.Repositories.Messaging;
using System.Linq;
using Xunit;

namespace Sevval.Messaging.Tests;

public class SendMessageCommandHandlerListingIdTests : IClassFixture<SqliteInMemoryDbFixture>
{
    private readonly SqliteInMemoryDbFixture _fixture;

    public SendMessageCommandHandlerListingIdTests(SqliteInMemoryDbFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Handle_WithListingId_SetsMessageListingId()
    {
        await using var context = _fixture.CreateContext();
        var ownerEmail = "owner@test.com";
        var ownerUser = TestDataFactory.CreateUser("owner-id", ownerEmail, "Kurumsal");
        var senderUser = TestDataFactory.CreateUser("sender-id", "sender@test.com");
        var listing = TestDataFactory.CreateListing(101, ownerEmail);

        context.Users.AddRange(ownerUser, senderUser);
        context.IlanBilgileri.Add(listing);
        await context.SaveChangesAsync();

        var repo = new MessageRepository(context);
        var handler = new SendMessageCommandHandler(repo, repo, new FixedDateTimeService(DateTime.UtcNow), context);

        var result = await handler.Handle(new SendMessageCommand
        {
            SenderId = senderUser.Id,
            RecipientId = ownerUser.Id,
            Body = "Test",
            ListingId = listing.Id
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var saved = context.Set<Sevval.Domain.Messaging.Message>().Single();
        Assert.Equal(listing.Id, saved.ListingId);
    }

    [Fact]
    public async Task Handle_ListingNotFound_ReturnsListingNotFound()
    {
        await using var context = _fixture.CreateContext();
        var repo = new MessageRepository(context);
        var handler = new SendMessageCommandHandler(repo, repo, new FixedDateTimeService(DateTime.UtcNow), context);

        var result = await handler.Handle(new SendMessageCommand
        {
            SenderId = "sender-id",
            RecipientId = "recipient-id",
            Body = "Test",
            ListingId = 999
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("ListingNotFound", result.Error);
    }

    [Fact]
    public async Task Handle_ListingOwnerNotFound_ReturnsListingOwnerNotFound()
    {
        await using var context = _fixture.CreateContext();
        var listing = TestDataFactory.CreateListing(202, "missing-owner@test.com");
        context.IlanBilgileri.Add(listing);
        await context.SaveChangesAsync();

        var repo = new MessageRepository(context);
        var handler = new SendMessageCommandHandler(repo, repo, new FixedDateTimeService(DateTime.UtcNow), context);

        var result = await handler.Handle(new SendMessageCommand
        {
            SenderId = "sender-id",
            RecipientId = "recipient-id",
            Body = "Test",
            ListingId = listing.Id
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("ListingOwnerNotFound", result.Error);
    }

    [Fact]
    public async Task Handle_ListingOwnerMismatch_ReturnsListingOwnerMismatch()
    {
        await using var context = _fixture.CreateContext();
        var ownerEmail = "owner2@test.com";
        var ownerUser = TestDataFactory.CreateUser("owner-2", ownerEmail, "Kurumsal");
        var listing = TestDataFactory.CreateListing(303, ownerEmail);
        context.Users.Add(ownerUser);
        context.IlanBilgileri.Add(listing);
        await context.SaveChangesAsync();

        var repo = new MessageRepository(context);
        var handler = new SendMessageCommandHandler(repo, repo, new FixedDateTimeService(DateTime.UtcNow), context);

        var result = await handler.Handle(new SendMessageCommand
        {
            SenderId = "sender-id",
            RecipientId = "someone-else",
            Body = "Test",
            ListingId = listing.Id
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("ListingOwnerMismatch", result.Error);
    }
}

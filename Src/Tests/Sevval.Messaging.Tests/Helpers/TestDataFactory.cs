using Sevval.Domain.Entities;
using Sevval.Domain.Messaging;

namespace Sevval.Messaging.Tests.Helpers;

public static class TestDataFactory
{
    public static ApplicationUser CreateUser(string id, string email, string userType = "Bireysel")
    {
        return new ApplicationUser
        {
            Id = id,
            Email = email,
            UserName = email,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            FirstName = "Test",
            LastName = "User",
            CompanyName = "Test Co",
            UserTypes = userType,
            City = "Istanbul",
            District = "Kadikoy",
            IPAddress = "127.0.0.1",
            Ucretlilik = "Active",
            IsSubscribed = "Active",
            IsActive = "Active"
        };
    }

    public static IlanModel CreateListing(int id, string ownerEmail)
    {
        return new IlanModel
        {
            Id = id,
            Email = ownerEmail,
            Title = "Test Listing",
            Description = "Listing description",
            Price = 100000,
            PricePerSquareMeter = 1000,
            Aidat = 0,
            TasınmazNumarasi = 0,
            Area = 100,
            Latitude = 0,
            Longitude = 0,
            GoruntulenmeSayisi = 0,
            GoruntulenmeTarihi = DateTime.UtcNow,
            Status = "active"
        };
    }

    public static Message CreateMessage(
        Guid id,
        string senderId,
        string recipientId,
        DateTime createdOnUtc,
        int? listingId = null)
    {
        return new Message(
            id,
            senderId,
            recipientId,
            "Test message",
            createdOnUtc,
            MessageStatus.Delivered,
            MessageType.Other)
        {
            ListingId = listingId
        };
    }
}

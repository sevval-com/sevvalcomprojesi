using Sevval.Domain.Entities;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementDetails
{
    public class GetAnnouncementDetailsQueryResponse
    {
        public IlanModel Announcement { get; set; }
        public List<string> Photos { get; set; } = new List<string>();
        public List<string> Videos { get; set; } = new List<string>();
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public List<RelatedAnnouncementDto> RelatedAnnouncements { get; set; } = new List<RelatedAnnouncementDto>();
        public CompanyDetailsDto? CompanyInfo { get; set; }
        public ConsultantDto ConsultantInfo { get; set; }

        public string Message { get; set; }
    }

    public class ConsultantDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicturePath { get; set; }
        public bool IsConsultant { get; set; }
        public string CompanyName { get; set; }
    }

    public class CompanyDetailsDto
    {
        public string CompanyName { get; set; }
        public string CompanyOwnerId { get; set; }
        public string Email { get; set; }
        public string CompanyOwnerProfilePicturePath { get; set; }
        public int TotalAnnouncementCount { get; set; }
        public DateTime? RegistrationDate { get; set; }

    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public DateTime CommentDate { get; set; }
        public int IlanId { get; set; }
    }

    public class RelatedAnnouncementDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal Area { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string ImagePath { get; set; }
    }

    
}

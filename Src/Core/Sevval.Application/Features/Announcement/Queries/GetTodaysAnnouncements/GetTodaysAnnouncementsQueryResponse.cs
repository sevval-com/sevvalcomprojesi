namespace Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements
{
    public class GetTodaysAnnouncementsQueryResponse
    {
        public int? Id { get; set; }
        public string? Category { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Price { get; set; }
        public double? Area { get; set; }
        public string? Sehir { get; set; }
        public string? Semt { get; set; }
        public string? MahalleKoy { get; set; }
        public DateTime? GirisTarihi { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? GoruntulenmeSayisi { get; set; }
        public string? Status { get; set; }
        public string? ProfilePicture { get; set; }
        public string? IlanNo { get; set; }
        public string? MulkTipi { get; set; }
        public string? OdaSayisi { get; set; }
        public string? BinaYasi { get; set; }
        public string? KatSayisi { get; set; }
        public string? BulunduguKat { get; set; }
        public string? TapuDurumu { get; set; }
        public string? Kimden { get; set; }
        public string? KonutDurumu { get; set; }
        public string? AdaNo { get; set; }
        public string? ParselNo { get; set; }
        public string? NetMetrekare { get; set; }

        public List<string>? ImagePaths { get; set; }
    }
}

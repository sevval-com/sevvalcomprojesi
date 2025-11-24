namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany
{
    public class GetAnnouncementsByCompanyQueryResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public string? Area { get; set; }
        public DateTime GirisTarihi { get; set; }
        public string? sehir { get; set; }
        public string? semt { get; set; }
        public string? mahalleKoy { get; set; }
        public string? AdaNo { get; set; }
        public string? ParselNo { get; set; }
        public string? NetMetrekare { get; set; }
        public string? OdaSayisi { get; set; }
        public string? BinaYasi { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IlanNo { get; set; }
        public string? Status { get; set; }
        public List<string> Images { get; set; }= new List<string>();

    }


}

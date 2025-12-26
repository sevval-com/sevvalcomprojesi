namespace Sevval.Application.Features.TempEstateVerification.Queries.GetTempEstateVerification
{
    public class GetTempEstateVerificationQueryResponse
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsSuccessfull { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
    }
}

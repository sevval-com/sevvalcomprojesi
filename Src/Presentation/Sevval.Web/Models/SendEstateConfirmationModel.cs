namespace sevvalemlak.Models
{
    public class SendEstateConfirmationModel
    {
        public string Email { get; set; }
        public EstateRegisterInfoModel Data { get; set; }
        public string ConfirmUrl { get; set; }
        public string RejectUrl { get; set; }
    }
}

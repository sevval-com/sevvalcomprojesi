namespace sevvalemlak.Dto
{
    public class SmsReportDTO
    {
        public string JobId { get; set; }
        public DateTime SendDate { get; set; }
        public string Message { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<SmsSendHistoryDTO> Records { get; set; } = new List<SmsSendHistoryDTO>();
    }

    public class SmsSendHistoryDTO
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }
}
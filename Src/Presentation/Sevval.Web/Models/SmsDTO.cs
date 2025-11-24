namespace sevvalemlak.Dto;

public class SmsDTO
{
    public List<UserSmsDTO> Users { get; set; } = new();
    public List<string> AvailableCities { get; set; } = new();

    public string SelectedPhone { get; set; }
}

public class UserSmsDTO
{
    public string Id { get; set; }
    public string Phone { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
}

public class NetGsmSmsRequest
{
    public string msgheader { get; set; }
    public MessageItem[] messages { get; set; }
    public string encoding { get; set; } = "TR";
    public string iysfilter { get; set; } = "0";
    public string partnercode { get; set; } = "";
}

public class MessageItem
{
    public string msg { get; set; }
    public string no { get; set; }
}

public class NetGsmResponse
{
    public string code { get; set; }
    public string jobid { get; set; }
    public string description { get; set; }
}
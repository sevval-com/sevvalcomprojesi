namespace Sevval.Application.Features.Province.GetProvincesWithDetail;

public class GetProvincesWithDetailQueryResponse
{

    public string Il { get; set; }
    public IlceDto[] Ilce { get; set; }


    public class IlceDto
    {
        public string Ilce { get; set; }
        public SemtDto[] Semt { get; set; }
    }

    public class SemtDto
    {
        public string PostaKodu { get; set; }
        public MahalleDto[] Mahalle { get; set; }
        public string Semt { get; set; }
    }

    public class MahalleDto
    {
        public string Mahalle { get; set; }
        public Coordinate[] coordinates { get; set; }
    }

    public class Coordinate
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }
}

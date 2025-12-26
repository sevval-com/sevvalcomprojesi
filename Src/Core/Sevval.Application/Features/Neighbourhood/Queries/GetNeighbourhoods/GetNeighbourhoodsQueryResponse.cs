namespace Sevval.Application.Features.Neighbourhood.Queries.GetNeighbourhoods
{
    public class GetNeighbourhoodsQueryResponse
    {
        public string PostCode { get; set; }  //posta kodu
        public string Name { get; set; }  //mahalle adı
        public string LocalityName { get; set; } //semt adı
    }
}

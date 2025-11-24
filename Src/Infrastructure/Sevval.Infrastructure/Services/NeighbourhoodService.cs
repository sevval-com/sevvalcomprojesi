using Sevval.Application.Features.Common;
using Sevval.Application.Features.Neighbourhood.Queries.GetNeighbourhoods;
using Sevval.Application.Features.Province.GetProvincesWithDetail;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Utilities;
using System.Net;
using System.Text.Json;

namespace Sevval.Infrastructure.Services;

public class NeighbourhoodService : INeighbourhoodService
{
    public async Task<ApiResponse<IList<GetNeighbourhoodsQueryResponse>>> GetNeighbourhoods(GetNeighbourhoodsQueryRequest request)
    {

        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Address.json");

            JsonDocument jsonDoc = await JsonReaderHelper.ReadJsonFileAsync(filePath);


            var provinces = jsonDoc.Deserialize<IList<GetProvincesWithDetailQueryResponse>>();

            var districts = provinces.FirstOrDefault(a => a.Il.ToLower() == request.ProvinceName.ToLower()).Ilce;

            var localities = districts.FirstOrDefault(a => a.Ilce.ToLower() == request.DistrictName.ToLower()).Semt;

            var neighbourhoods = localities.SelectMany(a => a.Mahalle).Select(a => new GetNeighbourhoodsQueryResponse
            {
                Name = a.Mahalle,
                LocalityName = localities.Where(s => s.Mahalle.Any(m => m.Mahalle == a.Mahalle)).FirstOrDefault()?.Semt,
                PostCode = localities.Where(s => s.Mahalle.Any(m => m.Mahalle == a.Mahalle)).FirstOrDefault()?.PostaKodu,

            }).ToList();


            return new ApiResponse<IList<GetNeighbourhoodsQueryResponse>>
            {
                Code = (int)HttpStatusCode.OK,
                Data = neighbourhoods,
                IsSuccessfull = true
            };
        }
        catch (Exception)
        {

            throw;
        }

    }
}

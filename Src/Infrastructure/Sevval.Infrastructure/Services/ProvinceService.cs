using Sevval.Application.Features.Common;
using Sevval.Application.Features.Province.GetProvinceByIdWithDetail;
using Sevval.Application.Features.Province.GetProvinces;
using Sevval.Application.Features.Province.GetProvincesWithDetail;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Utilities;
using System.Net;
using System.Text.Json;

namespace Sevval.Infrastructure.Services;

public class ProvinceService : IProvinceService
{

    public async Task<ApiResponse<IList<GetProvincesWithDetailQueryResponse>>> GetProvincesWithDetail(GetProvincesWithDetailQueryRequest request)
    {

        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Address.json");

            JsonDocument jsonDoc = await JsonReaderHelper.ReadJsonFileAsync(filePath);

            return new ApiResponse<IList<GetProvincesWithDetailQueryResponse>>
            {
                Code = (int)HttpStatusCode.OK,
                Data = jsonDoc.Deserialize<IList<GetProvincesWithDetailQueryResponse>>(),
                IsSuccessfull = true
            };
        }
        catch (Exception)
        {

            throw;
        }

    }
    public async Task<ApiResponse<GetProvinceByIdWithDetailQueryResponse>> GetProvinceByNameWithDetail(GetProvinceByIdWithDetailQueryRequest request)
    {

        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Address.json");

            JsonDocument jsonDoc = await JsonReaderHelper.ReadJsonFileAsync(filePath);


            var provinces = jsonDoc.Deserialize<IList<GetProvinceByIdWithDetailQueryResponse>>();

            var province = provinces.FirstOrDefault(a => a.Il.ToLower() == request.Name.ToLower());

            return new ApiResponse<GetProvinceByIdWithDetailQueryResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = province,
                IsSuccessfull = true
            };
        }
        catch (Exception)
        {

            throw;
        }

    }


    public async Task<ApiResponse<IList<GetProvincesQueryResponse>>> GetProvinces(GetProvincesQueryRequest request)
    {

        try
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Address.json");

            JsonDocument jsonDoc = await JsonReaderHelper.ReadJsonFileAsync(filePath);


            var provinces = jsonDoc.Deserialize<IList<GetProvincesWithDetailQueryResponse>>();

           

            return new ApiResponse<IList<GetProvincesQueryResponse>>
            {
                Code = (int)HttpStatusCode.OK,
                Data = provinces.Select(a=>new GetProvincesQueryResponse() {  Name = a.Il }).ToList(),
                IsSuccessfull = true
            };
        }
        catch (Exception)
        {

            throw;
        }

    }

    
}

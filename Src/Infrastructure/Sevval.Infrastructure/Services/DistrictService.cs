using Sevval.Application.Features.Common;
using Sevval.Application.Features.District.Commands.CreateDistrictCommands;
using Sevval.Application.Features.District.Queries.GetAllDistricts;
using Sevval.Application.Features.District.Queries.GetDistrictById;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities.Common;
using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AutoMapper;
using Sevval.Domain.Entities;
using Sevval.Application.Features.Province.GetProvinces;
using Sevval.Application.Features.Province.GetProvincesWithDetail;
using Sevval.Application.Utilities;
using System.Text.Json;

namespace Sevval.Infrastructure.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly IReadRepository<District> _readRepository;
        private readonly IWriteRepository<District> _writeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DistrictService(IReadRepository<District> readRepository, IMapper mapper, IUnitOfWork unitOfWork, IWriteRepository<District> writeRepository)
        {
            _readRepository = readRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _writeRepository = writeRepository;
        }

        public async Task<ApiResponse<GetDistrictByIdQueryResponse>> GetDistrict(GetDistrictByIdQueryRequest request)
        {
            var district = await _readRepository.GetAsync(x => x.Id == request.Id);

            if (district is null)
                return new ApiResponse<GetDistrictByIdQueryResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kayıt Bulunamadı"
                };

            return new ApiResponse<GetDistrictByIdQueryResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = _mapper.Map<GetDistrictByIdQueryResponse>(district),
                IsSuccessfull = true,
                Message = "Kayıt Getirildi"
            };
        }

        public async Task<ApiResponse<IList<GetAllDistrictsQueryResponse>>> GetAllDistricts(GetAllDistrictsQueryRequest request)
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "Address.json");

                JsonDocument jsonDoc = await JsonReaderHelper.ReadJsonFileAsync(filePath);


                var provinces = jsonDoc.Deserialize<IList<GetProvincesWithDetailQueryResponse>>();


                var province= provinces.FirstOrDefault(a => a.Il.ToLower() == request.ProvinceName.ToLower());

                var districts = province?.Ilce;

                return new ApiResponse<IList<GetAllDistrictsQueryResponse>>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = districts?.Select(a => new GetAllDistrictsQueryResponse() { Name = a.Ilce }).ToList(),
                    IsSuccessfull = true
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ApiResponse<CreateDistrictCommandResponse>> CreateDistrict(CreateDistrictCommandRequest request, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<District>(request);

            await _writeRepository.AddAsync(model);

            if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
            {
                var neighbourhood = _mapper.Map<CreateDistrictCommandResponse>(model);
                return new ApiResponse<CreateDistrictCommandResponse>
                {
                    Code = (int)HttpStatusCode.Created,
                    Data = neighbourhood,
                    IsSuccessfull = true,
                    Message = "Ekleme İşlemi Başarılı"
                };
            }

            return new ApiResponse<CreateDistrictCommandResponse>
            {
                Code = 400,
                Data = null,
                IsSuccessfull = false,
                Message = string.Empty
            };
        }
    }
}

using Sevval.Application.Features.Common;
using Sevval.Application.Features.FieldType.Queries.GetFieldTypes;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class FieldTypeService : IFieldTypeService
    {

        public async Task<ApiResponse<GetFieldTypesQueryResponse>> GetFieldTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var fieldTypes = new List<FieldTypeDTO>
                {
                    new FieldTypeDTO
                    {
                        Value = "Ayçiçeği",
                        DisplayName = "Ayçiçeği",
                        Description = "Ayçiçeği tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Bakla",
                        DisplayName = "Bakla",
                        Description = "Bakla tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Bamya",
                        DisplayName = "Bamya",
                        Description = "Bamya tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Bağ",
                        DisplayName = "Bağ",
                        Description = "Bağ alanı"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Bezelye",
                        DisplayName = "Bezelye",
                        Description = "Bezelye tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Çay",
                        DisplayName = "Çay",
                        Description = "Çay bahçesi"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Çilek",
                        DisplayName = "Çilek",
                        Description = "Çilek tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Fasulye",
                        DisplayName = "Fasulye",
                        Description = "Fasulye tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Ispanak",
                        DisplayName = "Ispanak",
                        Description = "Ispanak tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Kabak",
                        DisplayName = "Kabak",
                        Description = "Kabak tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Marul",
                        DisplayName = "Marul",
                        Description = "Marul tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Patlıcan",
                        DisplayName = "Patlıcan",
                        Description = "Patlıcan tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Salatalık",
                        DisplayName = "Salatalık",
                        Description = "Salatalık tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Sarımsak",
                        DisplayName = "Sarımsak",
                        Description = "Sarımsak tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Tarla+Bağ",
                        DisplayName = "Tarla+Bağ",
                        Description = "Karma tarla ve bağ alanı"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Tütün",
                        DisplayName = "Tütün",
                        Description = "Tütün tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Yer Fıstığı",
                        DisplayName = "Yer Fıstığı",
                        Description = "Yer fıstığı tarlası"
                    },
                    new FieldTypeDTO
                    {
                        Value = "Diğer",
                        DisplayName = "Diğer",
                        Description = "Diğer tarla türleri"
                    }
                };

                var response = new GetFieldTypesQueryResponse
                {
                    FieldTypes = fieldTypes,
                    Message = "Tarla tipleri başarıyla getirildi."
                };

                return new ApiResponse<GetFieldTypesQueryResponse>
                {
                    IsSuccessfull = true,
                    Message = "Tarla tipleri başarıyla getirildi.",
                    Data = response
                };
            }
            catch (System.Exception ex)
            {
                return new ApiResponse<GetFieldTypesQueryResponse>
                {
                    IsSuccessfull = false,
                    Message = "Tarla tipleri getirilirken bir hata oluştu: " + ex.Message
                };
            }
        }
    }
}

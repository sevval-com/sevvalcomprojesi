using Sevval.Application.Features.BusinessType.Queries.GetBusinessTypes;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Infrastructure.Services
{
    public class BusinessTypeService : IBusinessTypeService
    {

        public async Task<ApiResponse<List<GetBusinessTypesQueryResponse>>> GetBusinessTypesAsync(CancellationToken cancellationToken = default)
        {

            var businessTypes = new List<GetBusinessTypesQueryResponse>
                {
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Akaryakıt İstasyonu",
                        DisplayName = "Akaryakıt İstasyonu",
                        Description = "Akaryakıt istasyonu iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Apartman Dairesi",
                        DisplayName = "Apartman Dairesi",
                        Description = "Apartman dairesi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Aşevi",
                        DisplayName = "Aşevi",
                        Description = "Aşevi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Atölye",
                        DisplayName = "Atölye",
                        Description = "Atölye iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "AVM",
                        DisplayName = "AVM",
                        Description = "Alışveriş merkezi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Bakkal",
                        DisplayName = "Bakkal",
                        Description = "Bakkal iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Beyaz Eşya",
                        DisplayName = "Beyaz Eşya",
                        Description = "Beyaz eşya iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Bina",
                        DisplayName = "Bina",
                        Description = "Bina iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Büfe",
                        DisplayName = "Büfe",
                        Description = "Büfe iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Büro & Ofis",
                        DisplayName = "Büro & Ofis",
                        Description = "Büro ve ofis iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Çay Bahçesi",
                        DisplayName = "Çay Bahçesi",
                        Description = "Çay bahçesi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Çiftlik",
                        DisplayName = "Çiftlik",
                        Description = "Çiftlik iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Depo & Antrepo",
                        DisplayName = "Depo & Antrepo",
                        Description = "Depo ve antrepo iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Dershane",
                        DisplayName = "Dershane",
                        Description = "Dershane iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Düğün Salonu",
                        DisplayName = "Düğün Salonu",
                        Description = "Düğün salonu iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Dükkan & Mağaza",
                        DisplayName = "Dükkan & Mağaza",
                        Description = "Dükkan ve mağaza iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Eczane",
                        DisplayName = "Eczane",
                        Description = "Eczane iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Enerji Santrali",
                        DisplayName = "Enerji Santrali",
                        Description = "Enerji santrali iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Fabrika & Üretim Tesisi",
                        DisplayName = "Fabrika & Üretim Tesisi",
                        Description = "Fabrika ve üretim tesisi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Fırın & Tatlıcı",
                        DisplayName = "Fırın & Tatlıcı",
                        Description = "Fırın ve tatlıcı iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Garaj & Park Yeri",
                        DisplayName = "Garaj & Park Yeri",
                        Description = "Garaj ve park yeri iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Genel",
                        DisplayName = "Genel",
                        Description = "Genel iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Hamam & Sauna",
                        DisplayName = "Hamam & Sauna",
                        Description = "Hamam ve sauna iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Hastane",
                        DisplayName = "Hastane",
                        Description = "Hastane iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Huzurevi",
                        DisplayName = "Huzurevi",
                        Description = "Huzurevi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "İmalathane",
                        DisplayName = "İmalathane",
                        Description = "İmalathane iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "İş Hanı Katı & Ofisi",
                        DisplayName = "İş Hanı Katı & Ofisi",
                        Description = "İş hanı katı ve ofisi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Kahvehane",
                        DisplayName = "Kahvehane",
                        Description = "Kahvehane iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Kamu Binası",
                        DisplayName = "Kamu Binası",
                        Description = "Kamu binası iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Kantin",
                        DisplayName = "Kantin",
                        Description = "Kantin iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Kır & Kahvaltı Bahçesi",
                        DisplayName = "Kır & Kahvaltı Bahçesi",
                        Description = "Kır ve kahvaltı bahçesi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Kıraathane",
                        DisplayName = "Kıraathane",
                        Description = "Kıraathane iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Komple Bina",
                        DisplayName = "Komple Bina",
                        Description = "Komple bina iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Konfeksiyon",
                        DisplayName = "Konfeksiyon",
                        Description = "Konfeksiyon iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Market",
                        DisplayName = "Market",
                        Description = "Market iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Maden Ocağı",
                        DisplayName = "Maden Ocağı",
                        Description = "Maden ocağı iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Müstakil İş Yeri",
                        DisplayName = "Müstakil İş Yeri",
                        Description = "Müstakil iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Otopark",
                        DisplayName = "Otopark",
                        Description = "Otopark iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Pastane",
                        DisplayName = "Pastane",
                        Description = "Pastane iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Pazar Yeri",
                        DisplayName = "Pazar Yeri",
                        Description = "Pazar yeri iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Plaza",
                        DisplayName = "Plaza",
                        Description = "Plaza iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Plaza Katı & Ofisi",
                        DisplayName = "Plaza Katı & Ofisi",
                        Description = "Plaza katı ve ofisi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Radyo İstasyonu & TV Kanalı",
                        DisplayName = "Radyo İstasyonu & TV Kanalı",
                        Description = "Radyo istasyonu ve TV kanalı iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Restoran & Lokanta",
                        DisplayName = "Restoran & Lokanta",
                        Description = "Restoran ve lokanta iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Rezidans",
                        DisplayName = "Rezidans",
                        Description = "Rezidans iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Rezidans Katı & Ofisi",
                        DisplayName = "Rezidans Katı & Ofisi",
                        Description = "Rezidans katı ve ofisi iş yeri"
                    },
                    new GetBusinessTypesQueryResponse
                    {
                        Value = "Sağlık Merkezi",
                        DisplayName = "Sağlık Merkezi",
                        Description = "Sağlık merkezi iş yeri"
                    }
                };

            return new ApiResponse<List<GetBusinessTypesQueryResponse>>
            {
                Data = businessTypes,
                IsSuccessfull = true,
                Message = "İş yeri tipleri başarıyla getirildi."

            };


        }
    }
}

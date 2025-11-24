using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Dtos.FormDatas;
using Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;
using Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;
using Sevval.Persistence.Context;
using sevvalemlak.csproj.ClientServices.SalesRequestService;

namespace YourProject.Controllers
{
    public class FormController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISalesRequestClientService _salesRequestClientService;

        public FormController(ApplicationDbContext dbContext, ISalesRequestClientService salesRequestClientService)
        {
            _dbContext = dbContext;
            _salesRequestClientService = salesRequestClientService;
        }

        public IActionResult MulkumuSatmakIstiyorum()
        {
            return View();
        }

        public IActionResult YatirimYapmakIstiyorum()
        {
            return View();
        }


        

        [HttpPost]
        [Route("Form/SendMail")]
        public async Task<IActionResult> SendMail([FromBody] FormData formData)
        {
            // Form verisini veritabanına kaydet ve mail gönder
            var response = await _salesRequestClientService.CreateSalesRequestAsync(new CreateSalesRequestCommandRequest
            {
                SelectCategory = formData.selectCategory,
                SelectStatus = formData.selectStatus,
                Rooms = formData.Rooms,
                Area = formData.Area,
                BuildingAge = formData.BuildingAge,
                Floor = formData.Floor,
                Bathrooms = formData.Bathrooms,
                HeatingSystem = formData.HeatingSystem,
                Parking = formData.Parking,
                Price = formData.Price,
                ResidentialCity = formData.ResidentialCity,
                ResidentialDistrict = formData.ResidentialDistrict,
                ResidentialVillage = formData.ResidentialVillage,
                LandAda = formData.LandAda,
                LandParsel = formData.LandParsel,
                LandArea = formData.LandArea,
                Slope = formData.Slope,
                RoadCondition = formData.RoadCondition,
                DistanceToSettlement = formData.DistanceToSettlement,
                ZoningStatus = formData.ZoningStatus,
                LandCity = formData.LandCity,
                LandDistrict = formData.LandDistrict,
                LandVillage = formData.LandVillage,
                LandPrice = formData.LandPrice,
                FirstName = formData.FirstName,
                LastName = formData.LastName,
                Email = formData.Email,
                Phone = formData.Phone,
                LivingCity = formData.LivingCity

            }, CancellationToken.None);

            if (response.IsSuccessfull)
            {
                return Ok(new { message = "Mailler gönderildi ve form verileri kayıt edildi." });

            }

            return BadRequest(new { error = response.Message });


        }


        [HttpPost]
        [Route("Form/SendMailInvestment")]
        public async Task<IActionResult> SendMailInvestment([FromBody] InvestmentDto formData)
        {
            // Form verisini veritabanına kaydet ve mail gönder
            var response = await _salesRequestClientService.CreateInvestmentRequestAsync(new CreateInvestmentRequestCommandRequest
            {
                SelectCategory = formData.selectCategory,
                SelectStatus = formData.selectStatus,
                Rooms = formData.Rooms,
                Area = formData.Area,
                BuildingAge = formData.BuildingAge,
                Floor = formData.Floor,
                Bathrooms = formData.Bathrooms,
                HeatingSystem = formData.HeatingSystem,
                Parking = formData.Parking,
                Price = formData.Price,
                ResidentialCity = formData.ResidentialCity,
                ResidentialDistrict = formData.ResidentialDistrict,
                ResidentialVillage = formData.ResidentialVillage,
                LandArea = formData.LandArea,
                Slope = formData.Slope,
                RoadCondition = formData.RoadCondition,
                DistanceToSettlement = formData.DistanceToSettlement,
                ZoningStatus = formData.ZoningStatus,
                LandCity = formData.LandCity,
                LandDistrict = formData.LandDistrict,
                LandVillage = formData.LandVillage,
                LandPrice = formData.LandPrice,
                FirstName = formData.FirstName,
                LastName = formData.LastName,
                Email = formData.Email,
                Phone = formData.Phone,
                LivingCity = formData.LivingCity,
                MaxBudget = formData.MaxBudget,
                MinBudget = formData.MinBudget

            }, CancellationToken.None);

            if (response.IsSuccessfull)
            {
                return Ok(new { message = "Mailler gönderildi ve form verileri kayıt edildi." });

            }

            return BadRequest(new { error = response.Message });


        }

    }
}

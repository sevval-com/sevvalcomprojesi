using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.BalconyOptions.Queries.GetBalconyOptions;
using Sevval.Application.Features.BathroomOptions.Queries.GetBathroomOptions;
using Sevval.Application.Features.BuildingAge.Queries.GetBuildingAges;
using Sevval.Application.Features.DistanceToSettlementOptions.Queries.GetDistanceToSettlementOptions;
using Sevval.Application.Features.FloorOptions.Queries.GetFloorOptions;
using Sevval.Application.Features.HeatingSystemOptions.Queries.GetHeatingSystemOptions;
using Sevval.Application.Features.RoadConditionOptions.Queries.GetRoadConditionOptions;
using Sevval.Application.Features.RoomOptions.Queries.GetRoomOptions;
using Sevval.Application.Features.SlopeOptions.Queries.GetSlopeOptions;
using Sevval.Application.Features.ZoningStatusOptions.Queries.GetZoningStatusOptions;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SearchOptionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchOptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Bina yaşı seçeneklerini getirir
        /// </summary>
        /// <returns>Bina yaşı seçenekleri listesi</returns>
        [HttpGet(GetBuildingAgesQueryRequest.Route)]
        [SwaggerOperation(Summary = "Bina yaşı seçenekleri", Description = "Bina yaşı seçeneklerini getirir (0, 1, 2, 3, 4, 5-10 arası, vb.)")]
        public async Task<IActionResult> GetBuildingAges([FromQuery] GetBuildingAgesQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response.IsSuccessfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Yerleşim yerine uzaklık seçeneklerini getirir
        /// </summary>
        /// <returns>Yerleşim yerine uzaklık seçenekleri listesi</returns>
        [HttpGet(GetDistanceToSettlementOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Yerleşim yerine uzaklık seçeneklerini getirir", Description = "Bu endpoint, yerleşim yerine uzaklık seçeneklerini döner.")]
        public async Task<IActionResult> GetDistanceToSettlementOptions([FromQuery] GetDistanceToSettlementOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Kat seçeneklerini getirir
        /// </summary>
        /// <returns>Kat seçenekleri listesi</returns>
        [HttpGet(GetFloorOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Kat seçeneklerini getirir", Description = "Bu endpoint, kat seçeneklerini döner.")]
        public async Task<IActionResult> GetFloorOptions([FromQuery] GetFloorOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Isıtma sistemi seçeneklerini getirir
        /// </summary>
        /// <returns>Isıtma sistemi seçenekleri listesi</returns>
        [HttpGet(GetHeatingSystemOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Isıtma sistemi seçeneklerini getirir", Description = "Bu endpoint, ısıtma sistemi seçeneklerini döner.")]
        public async Task<IActionResult> GetHeatingSystemOptions([FromQuery] GetHeatingSystemOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Yol durumu seçeneklerini getirir
        /// </summary>
        /// <returns>Yol durumu seçenekleri listesi</returns>
        [HttpGet(GetRoadConditionOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Yol durumu seçeneklerini getirir", Description = "Bu endpoint, yol durumu seçeneklerini döner.")]
        public async Task<IActionResult> GetRoadConditionOptions([FromQuery] GetRoadConditionOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// İmar durumu seçeneklerini getirir
        /// </summary>
        /// <returns>İmar durumu seçenekleri listesi</returns>
        [HttpGet(GetZoningStatusOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "İmar durumu seçeneklerini getirir", Description = "Bu endpoint, imar durumu seçeneklerini döner.")]
        public async Task<IActionResult> GetZoningStatusOptions([FromQuery] GetZoningStatusOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Oda sayısı seçeneklerini getirir
        /// </summary>
        /// <returns>Oda sayısı seçenekleri listesi</returns>
        [HttpGet(GetRoomOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Oda sayısı seçenekleri", Description = "Oda sayısı seçeneklerini getirir (Stüdyo, 1+1, 2+1, vb.)")]
        public async Task<IActionResult> GetRoomOptions([FromQuery] GetRoomOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (response.IsSuccessfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        /// <summary>
        /// Eğim seçeneklerini getirir
        /// </summary>
        /// <returns>Eğim seçenekleri listesi</returns>
        [HttpGet(GetSlopeOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Eğim seçeneklerini getirir", Description = "Bu endpoint, eğim seçeneklerini döner.")]
        public async Task<IActionResult> GetSlopeOptions([FromQuery] GetSlopeOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Balkon seçeneklerini getirir
        /// </summary>
        /// <returns>Balkon seçenekleri listesi</returns>
        [HttpGet(GetBalconyOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Balkon seçeneklerini getirir", Description = "Bu endpoint, balkon seçeneklerini listelemek için kullanılır.")]
        public async Task<IActionResult> GetBalconyOptions([FromQuery]GetBalconyOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Banyo sayısı seçeneklerini getirir
        /// </summary>
        /// <returns>Banyo sayısı seçenekleri listesi</returns>
        [HttpGet(GetBathroomOptionsQueryRequest.Route)]
        [SwaggerOperation(Summary = "Banyo sayısı seçenekleri", Description = "Banyo sayısı seçeneklerini getirir (1, 2, 3, vb.)")]
        public async Task<IActionResult> GetBathroomOptions([FromQuery] GetBathroomOptionsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}

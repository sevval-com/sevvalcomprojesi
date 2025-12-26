using MediatR;
using Microsoft.Extensions.Logging;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.TempEstateVerification.Commands.DeleteTempEstateVerification;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.RejectEstate
{
    public class RejectEstateCommandHandler : IRequestHandler<RejectEstateCommandRequest, ApiResponse<RejectEstateCommandResponse>>
    {
        private readonly IUserService _userService;
        private readonly ITempEstateVerificationService _tempEstateVerificationService;
        private readonly ILogger<RejectEstateCommandHandler> _logger;

        public RejectEstateCommandHandler(
            IUserService userService,
            ITempEstateVerificationService tempEstateVerificationService,
            ILogger<RejectEstateCommandHandler> logger)
        {
            _userService = userService;
            _tempEstateVerificationService = tempEstateVerificationService;
            _logger = logger;
        }

        public async Task<ApiResponse<RejectEstateCommandResponse>> Handle(RejectEstateCommandRequest request, CancellationToken cancellationToken)
        {

            var tempEstate = await _tempEstateVerificationService.GetTempEstateVerification(new TempEstateVerification.Queries.GetTempEstateVerification.GetTempEstateVerificationQueryRequest
            {
                Email = request.Email,
                Code = request.Token

            }, cancellationToken);

            if (!tempEstate.IsSuccessfull || tempEstate.Data == null)
            {
                return new ApiResponse<RejectEstateCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = "Geçersiz veya süresi dolmuş doğrulama kodu."
                };
            }

            // Reject the estate registration
            var response = await _userService.RejectEstate(request,cancellationToken);

            if (response.IsSuccessfull)
            {
                await _tempEstateVerificationService.DeleteTempEstateVerification(new DeleteTempEstateVerificationCommandRequest
                {
                    Id = tempEstate.Data.Id

                }, cancellationToken);
            }

            return response;

        }
    }
}

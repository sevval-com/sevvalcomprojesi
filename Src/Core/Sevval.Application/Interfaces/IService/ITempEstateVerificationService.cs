using Sevval.Application.Features.Common;
using Sevval.Application.Features.TempEstateVerification.Commands.CreateTempEstateVerification;
using Sevval.Application.Features.TempEstateVerification.Commands.DeleteTempEstateVerification;
using Sevval.Application.Features.TempEstateVerification.Commands.VerifyTempEstateVerification;
using Sevval.Application.Features.TempEstateVerification.Queries.GetTempEstateVerification;
using Sevval.Domain.Entities;

namespace Sevval.Application.Interfaces.IService
{
    public interface ITempEstateVerificationService
    {
        Task<ApiResponse<CreateTempEstateVerificationCommandResponse>> CreateTempEstateVerification(CreateTempEstateVerificationCommandRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<DeleteTempEstateVerificationCommandResponse>> DeleteTempEstateVerification(DeleteTempEstateVerificationCommandRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<GetTempEstateVerificationQueryResponse>> GetTempEstateVerification(GetTempEstateVerificationQueryRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<VerifyTempEstateVerificationCommandResponse>> VerifyTempEstateVerification(VerifyTempEstateVerificationCommandRequest request, CancellationToken cancellationToken = default);
    }
}

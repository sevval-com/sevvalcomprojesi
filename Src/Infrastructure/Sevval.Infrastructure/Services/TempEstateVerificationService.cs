using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Sevval.Application.Constants;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.TempEstateVerification.Commands.CreateTempEstateVerification;
using Sevval.Application.Features.TempEstateVerification.Commands.DeleteTempEstateVerification;
using Sevval.Application.Features.TempEstateVerification.Commands.VerifyTempEstateVerification;
using Sevval.Application.Features.TempEstateVerification.Queries.GetTempEstateVerification;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Sevval.Infrastructure.Services
{
    public class TempEstateVerificationService : ITempEstateVerificationService
    {
        private readonly IReadRepository<TempEstateVerification> _readRepository;
        private readonly IWriteRepository<TempEstateVerification> _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TempEstateVerificationService(
            IReadRepository<TempEstateVerification> readRepository,
            IWriteRepository<TempEstateVerification> writeRepository,
            IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<CreateTempEstateVerificationCommandResponse>> CreateTempEstateVerification(CreateTempEstateVerificationCommandRequest request, CancellationToken cancellationToken = default)
        {
            try
            {

                string uniqueData = $"{request.Email}-{DateTime.UtcNow.Ticks}";

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GeneralConstants.SecretKey));

                string verificationCode = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(uniqueData)))
                    .Where(c => char.IsLetterOrDigit(c)).Aggregate("", (s, c) => s + c).Substring(0, 20);

                verificationCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(verificationCode));



                // Set expiration time (e.g., 24 hours from now)
                DateTime expiration = DateTime.UtcNow.AddHours(24);

                // Check if a verification code already exists for this email
                var existingVerification = await _readRepository.GetAsync(v => v.Email == request.Email);

                if (existingVerification is not null)
                {
                    // Update existing verification
                    existingVerification.Code = verificationCode;
                    existingVerification.Expiration = expiration;
                    await _writeRepository.UpdateAsync(existingVerification);
                }
                else
                {
                    // Create new verification
                    var verification = new TempEstateVerification
                    {
                        Email = request.Email,
                        Code = verificationCode,
                        Expiration = expiration
                    };

                    await _writeRepository.AddAsync(verification);
                    existingVerification = verification;
                }

                if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
                {
                    return new ApiResponse<CreateTempEstateVerificationCommandResponse>
                    {
                        Data = new CreateTempEstateVerificationCommandResponse
                        {
                            IsSuccessfull = true,
                            VerificationCode = verificationCode
                        },
                        IsSuccessfull = true,
                        Message = "Verification code created successfully"
                    };
                }


            }
            catch (Exception ex)
            {
                return new ApiResponse<CreateTempEstateVerificationCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = $"Error creating verification code: {ex.Message}"
                };
            }

            return new ApiResponse<CreateTempEstateVerificationCommandResponse>
            {
                IsSuccessfull = false,
                Message = $"Error creating verification code"
            };
        }

        public async Task<ApiResponse<VerifyTempEstateVerificationCommandResponse>> VerifyTempEstateVerification(VerifyTempEstateVerificationCommandRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var verification = await _readRepository.FindAsync(v => v.Email == request.Email, true);

                if (verification == null)
                {
                    return new ApiResponse<VerifyTempEstateVerificationCommandResponse>
                    {
                        Data = new VerifyTempEstateVerificationCommandResponse { IsSuccessfull = false },
                        IsSuccessfull = false,
                        Message = "No verification code found for this email"
                    };
                }

                if (verification.Expiration < DateTime.UtcNow)
                {
                    return new ApiResponse<VerifyTempEstateVerificationCommandResponse>
                    {
                        Data = new VerifyTempEstateVerificationCommandResponse { IsSuccessfull = false },
                        IsSuccessfull = false,
                        Message = "Verification code has expired"
                    };
                }

                if (verification.Code != request.Code)
                {
                    return new ApiResponse<VerifyTempEstateVerificationCommandResponse>
                    {
                        Data = new VerifyTempEstateVerificationCommandResponse { IsSuccessfull = false },
                        IsSuccessfull = false,
                        Message = "Invalid verification code"
                    };
                }

                return new ApiResponse<VerifyTempEstateVerificationCommandResponse>
                {
                    Data = new VerifyTempEstateVerificationCommandResponse { IsSuccessfull = true },
                    IsSuccessfull = true,
                    Message = "Verification successful"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<VerifyTempEstateVerificationCommandResponse>
                {
                    Data = new VerifyTempEstateVerificationCommandResponse { IsSuccessfull = false },
                    IsSuccessfull = false,
                    Message = $"Error verifying code"
                };
            }
        }

        public async Task<ApiResponse<GetTempEstateVerificationQueryResponse>> GetTempEstateVerification(GetTempEstateVerificationQueryRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var verification = await _readRepository.GetAsync(v => v.Email == request.Email && v.Expiration > DateTime.Now && v.Code == request.Code);

                if (verification is null)
                {
                    return new ApiResponse<GetTempEstateVerificationQueryResponse>
                    {
                        IsSuccessfull = false,
                        Message = "Bu doðrulama linki geçersiz veya süresi dolmuþ."
                        ,
                        Data = new GetTempEstateVerificationQueryResponse() { IsSuccessfull = false },
                    };
                }

                return new ApiResponse<GetTempEstateVerificationQueryResponse>
                {
                    Data = new GetTempEstateVerificationQueryResponse
                    {
                        Expiration = verification.Expiration,
                        Email = verification.Email,
                        Code = verification.Code,
                        IsSuccessfull = true,
                        Message = "Verification found",
                        Id = verification.Id

                    },
                    IsSuccessfull = true,
                    Message = "Verification found"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetTempEstateVerificationQueryResponse>
                {
                    IsSuccessfull = false,
                    Message = $"Error retrieving verification"
                };
            }
        }

        public async Task<ApiResponse<DeleteTempEstateVerificationCommandResponse>> DeleteTempEstateVerification(DeleteTempEstateVerificationCommandRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var verification = await _readRepository.GetAsync(v => v.Id == request.Id);

                if (verification == null)
                {
                    return new ApiResponse<DeleteTempEstateVerificationCommandResponse>
                    {
                        Data = new DeleteTempEstateVerificationCommandResponse { IsSuccessfull = false },
                        IsSuccessfull = false,
                        Message = "Bu doðrulama linki geçersiz veya süresi dolmuþ."
                    };
                }

                await _writeRepository.DeleteAsync(verification);

                if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
                {
                    return new ApiResponse<DeleteTempEstateVerificationCommandResponse>
                    {
                        Data = new DeleteTempEstateVerificationCommandResponse { IsSuccessfull = true },
                        IsSuccessfull = true,
                        Message = "Doðrulama silme iþlemi baþarýlý"
                    };
                }


            }
            catch (Exception ex)
            {
                return new ApiResponse<DeleteTempEstateVerificationCommandResponse>
                {
                    Data = new DeleteTempEstateVerificationCommandResponse { IsSuccessfull = false },
                    IsSuccessfull = false,
                    Message = $"Error deleting verification"
                };
            }

            return new ApiResponse<DeleteTempEstateVerificationCommandResponse>
            {
                Data = new DeleteTempEstateVerificationCommandResponse { IsSuccessfull = false },
                IsSuccessfull = false,
                Message = $"Doðrulama iþlemi baþarýsýz"
            };
        }

    }
}

using AutoMapper;
using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Constants;
using Sevval.Application.Dtos.Email;
using Sevval.Application.Dtos.Front.Auth;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Commands.AddUser;
using Sevval.Application.Features.User.Commands.ConfirmEstate;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.DeleteUser;
using Sevval.Application.Features.User.Commands.ForgottenPassword;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.LoginWithSocialMedia;
using Sevval.Application.Features.User.Commands.RejectEstate;
using Sevval.Application.Features.User.Commands.SendNewCode;
using Sevval.Application.Features.User.Commands.UpdateUser;
using Sevval.Application.Features.User.Commands.UpdateUserProfile;
using Sevval.Application.Features.User.Commands.RefreshToken;
using Sevval.Application.Features.User.Commands.IndividualUpdate;
using Sevval.Application.Features.User.Commands.CorporateUpdate;
using Sevval.Application.Features.User.Commands.UpdatePassword;
using Sevval.Application.Features.User.Queries.CheckUserExists;
using Sevval.Application.Features.User.Queries.GetAllUsers;
using Sevval.Application.Features.User.Queries.GetUserById;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Utilities;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;

namespace Sevval.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IWriteRepository<ApplicationUser> _writeRepository;
        private readonly IReadRepository<ApplicationUser> _readRepository;
        private readonly IWriteRepository<UserRefreshToken> _writeUserRefreshTokenRepository;
        private readonly IReadRepository<UserRefreshToken> _readUserRefreshTokenRepository;
        private readonly IWriteRepository<ForgettenPassword> _forgettenPasswordWriteRepository;
        private readonly IReadRepository<ForgettenPassword> _forgettenPasswordReadRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEMailService _emailService;
        private readonly IEMailService _eMailService;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenService _tokenService;


        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private string imgUrl = "";

        [Obsolete]
        public UserService(IWriteRepository<ApplicationUser> writeRepository, IReadRepository<ApplicationUser> readRepository,
         IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager,
         IWriteRepository<ForgettenPassword> forgettenPasswordWriteRepository, //IMailManagerService mailManagerService, 
         IReadRepository<ForgettenPassword> forgettenPasswordReadRepository,
         IHostingEnvironment hostingEnvironment, IEMailService emailService,
         IWriteRepository<UserRefreshToken> writeUserRefreshTokenRepository,
         IReadRepository<UserRefreshToken> readUserRefreshTokenRepository, ITokenService tokenService,
         ApplicationDbContext context)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _forgettenPasswordWriteRepository = forgettenPasswordWriteRepository;
            // _mailManagerService = mailManagerService;
            _forgettenPasswordReadRepository = forgettenPasswordReadRepository;

            _hostingEnvironment = hostingEnvironment;
            imgUrl = _hostingEnvironment.WebRootPath + "\\userImages\\";
            _emailService = emailService;
            _eMailService = emailService;
            _context = context;
            _writeUserRefreshTokenRepository = writeUserRefreshTokenRepository;
            _readUserRefreshTokenRepository = readUserRefreshTokenRepository;

            SeedDefaultRoles().GetAwaiter().GetType();
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<IList<GetAllUsersQueryResponse>>> GetAllUsers(GetAllUsersQueryRequest request)
        {
            var users = _readRepository.Queryable();

            if (!string.IsNullOrWhiteSpace(request.Username)) users = users.Where(a => a.UserName.StartsWith(request.Username));
            if (!string.IsNullOrWhiteSpace(request.Email)) users = users.Where(a => a.Email.StartsWith(request.Email));
            if (!string.IsNullOrWhiteSpace(request.Name)) users = users.Where(a => a.FirstName.StartsWith(request.Name));
            if (!string.IsNullOrWhiteSpace(request.Surname)) users = users.Where(a => a.LastName.StartsWith(request.Surname));



            var result = await PaginatedList<ApplicationUser>.CreateAsync(users.OrderByDescending(a => a.Id), request.PageNumber, request.PageSize);

            if (result.TotalItems == 0)
                return new ApiResponse<IList<GetAllUsersQueryResponse>>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kayıt Bulunamadı",
                };

            return new ApiResponse<IList<GetAllUsersQueryResponse>>
            {
                Code = (int)HttpStatusCode.OK,
                Data = _mapper.Map<List<GetAllUsersQueryResponse>>(result),
                IsSuccessfull = true,
                Message = "Kayıt Getirildi",
                Meta = new MetaData
                {
                    Pagination = new Pagination
                    {
                        TotalItem = result.TotalItems,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalPage = result.TotalPages,

                    }
                }
            };
        }




        public async Task<ApiResponse<LoginUserDto>> LoginAsync(LoginDto LoginDto)
        {

            //LoginDto.Password = Md5Service.Encrypt(LoginDto.Password);
            LoginUserDto adminUserSessionModel = null;
            var user = await _readRepository.GetAsync(a => a.Email == LoginDto.Email && a.PasswordHash == LoginDto.Password);
            if (user is null)
                return new ApiResponse<LoginUserDto>
                {
                    Code = 404,
                    Data = new LoginUserDto(),
                    IsSuccessfull = false,
                    Message = "Kullanıcı bilgilerini yanlış girdiniz."
                };


            /* adminUserSessionModel = new LoginUserDto
             {
                 ImagePath = string.IsNullOrEmpty(user.ImagePath) == false ? user.ImagePath : "-",
                 Mail = user.Mail,
                 SystemAuthTime = DateTime.Now,
                 TypeId = user.TypeId,
                 UserId = Md5Service.Encrypt(user.Id.ToString()),
                 Username = user.Name + " " + user.Surname,
                 Token = user.Token,



             };

             if (user.TokenExpireDate == null ||
                 string.IsNullOrEmpty(user.Token) == true || user.TokenExpireDate < DateTime.Now)
             {
                 var accessToken = _tokenService.CreateToken(user.Id.ToString(), user.Mail);
                 user.Token = accessToken.Token;
                 user.TokenExpireDate = accessToken.Expiration;


                 var result = await _writeRepository.UpdateAsync(user);

                 return new ApiResponse<LoginUserDto>
                 {
                     IsSuccessfull = true,
                     Message = "Giriş Başarılı",
                     Code = 200,
                     Data = adminUserSessionModel
                 };
             }

             */
            return new ApiResponse<LoginUserDto>
            {
                IsSuccessfull = true,
                Message = "Giriş Başarılı",
                Code = 200,
                Data = adminUserSessionModel
            };

        }

        //bireysel
        public async Task<ApiResponse<IndividualRegisterCommandResponse>> IndividualRegister(IndividualRegisterCommandRequest request, CancellationToken cancellationToken)
        {

            var user = _mapper.Map<ApplicationUser>(request);

            user.UserTypes = "Bireysel";
            user.IsSubscribed = "ücretsiz";
            user.IsActive = "active";
            user.UserOrder = await GetNextUserOrder("Bireysel"); // ✅ Otomatik numara ata
            user.RegistrationDate = DateTime.Now;

            if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                var fileName = Path.GetFileName(request.ProfilePicture.FileName);
                var uploadsDirectory = Path.Combine(GeneralConstants.WwwRootPath, "Images", "UserImages");

                // Uploads klasörü yoksa oluştur
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ProfilePicture.CopyToAsync(stream);
                }

                // Yüklenen fotoğrafın yolunu kaydet
                user.ProfilePicturePath = "/Images/UserImages/" + fileName;
            }
            else
            {
                // Kullanıcı fotoğrafı yüklemediyse varsayılan yolu ata
                user.ProfilePicturePath = "/Images/Common/defaultUser.png";
            }

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    string encodedCode = WebUtility.UrlEncode(Md5Service.Encrypt(code));

                    string url = $"{GeneralConstants.BaseUrl}/VerifyEmail?email={HttpUtility.UrlEncode(request.Email)}&code={encodedCode}";

                    var emailResult = await _emailService.SendVerificationEmailAsync(new SendVerifyEmailDto
                    {
                        Code = encodedCode,
                        Email = user?.Email,
                        Link = url,
                    });

                    await _userManager.AddToRoleAsync(user, "Bireysel");

                    if (!emailResult)
                    {
                        return new ApiResponse<IndividualRegisterCommandResponse>
                        {
                            Code = (int)HttpStatusCode.BadRequest,
                            Data = _mapper.Map<IndividualRegisterCommandResponse>(user),
                            IsSuccessfull = false,
                            Message = "Ekleme İşlemi Başarılı fakat mail gönderilemedi"
                        };
                    }


                    return new ApiResponse<IndividualRegisterCommandResponse>
                    {
                        Code = (int)HttpStatusCode.OK,
                        Data = _mapper.Map<IndividualRegisterCommandResponse>(user),
                        IsSuccessfull = true,
                        Message = "Ekleme İşlemi Başarılı"
                    };
                }

                if (result.Errors.Any(a => a.Code == "DuplicateUserName" || a.Code == "DuplicateEmail"))
                    return new ApiResponse<IndividualRegisterCommandResponse>
                    {
                        Code = 400,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Aynı e-posta adresi ile kayıt olunduğu için tekrar kayıt oluşturulamıyor!"
                    };

                return new ApiResponse<IndividualRegisterCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = string.Join(", ", result.Errors.Select(a => a.Description).ToList())
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<IndividualRegisterCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Bir hata oluştu"
                };
            }
        }

        // Individual Update Method
        public async Task<ApiResponse<IndividualUpdateCommandResponse>> IndividualUpdate(IndividualUpdateCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the existing user
                var user = await _readRepository.GetAsync(x => x.Id == request.Id, EnableTracking: true);

                if (user is null)
                {
                    return new ApiResponse<IndividualUpdateCommandResponse>
                    {
                        Code = 404,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Kullanıcı bulunamadı."
                    };
                }

                user = _mapper.Map(request, user);


                // Check if user is individual type
                if (user.UserTypes != "Bireysel")
                {
                    return new ApiResponse<IndividualUpdateCommandResponse>
                    {
                        Code = 400,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Bu kullanıcı bireysel kullanıcı değil."
                    };
                }

                // Handle profile picture update
                if (request.RemoveProfilePicture)
                {
                    // Remove existing profile picture and set to default
                    if (!string.IsNullOrEmpty(user.ProfilePicturePath) && user.ProfilePicturePath != "/Images/Common/defaultUser.png")
                    {
                        var oldFilePath = Path.Combine(GeneralConstants.WwwRootPath, user.ProfilePicturePath.TrimStart('/'));
                        if (File.Exists(oldFilePath) && user.ProfilePicturePath != "/Images/Common/defaultUser.png")
                        {
                            File.Delete(oldFilePath);
                        }
                    }
                    user.ProfilePicturePath = "/Images/Common/defaultUser.png";
                }
                else if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
                {
                    // Delete old profile picture if it exists and is not default
                    if (!string.IsNullOrEmpty(user.ProfilePicturePath) && user.ProfilePicturePath != "/Images/Common/defaultUser.png")
                    {
                        var oldFilePath = Path.Combine(GeneralConstants.WwwRootPath, user.ProfilePicturePath.TrimStart('/'));
                        if (File.Exists(oldFilePath) && user.ProfilePicturePath != "/Images/Common/defaultUser.png")
                        {
                            File.Delete(oldFilePath);
                        }
                    }

                    // Upload new profile picture
                    var fileName = Path.GetFileName(request.ProfilePicture.FileName);
                    var uploadsDirectory = Path.Combine(GeneralConstants.WwwRootPath, "Images", "UserImages");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.ProfilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePicturePath = "/Images/UserImages/" + fileName;
                }





                // Update password if provided
                if (!string.IsNullOrEmpty(request.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);

                    if (!passwordResult.Succeeded)
                    {
                        return new ApiResponse<IndividualUpdateCommandResponse>
                        {
                            Code = 400,
                            Data = null,
                            IsSuccessfull = false,
                            Message = "Şifre güncellenirken hata oluştu: " + string.Join(", ", passwordResult.Errors.Select(e => e.Description))
                        };
                    }
                }

                // Update the user
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return new ApiResponse<IndividualUpdateCommandResponse>
                    {
                        Code = 200,
                        Data = new IndividualUpdateCommandResponse
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            ProfilePicturePath = user.ProfilePicturePath,
                            UpdatedDate = DateTime.Now
                        },
                        IsSuccessfull = true,
                        Message = "Bireysel kullanıcı başarıyla güncellendi."
                    };
                }

                return new ApiResponse<IndividualUpdateCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = string.Join(", ", result.Errors.Select(a => a.Description).ToList())
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<IndividualUpdateCommandResponse>
                {
                    Code = 500,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Güncelleme sırasında bir hata oluştu: " + e.Message
                };
            }
        }

        public async Task<ApiResponse<CorporateRegisterCommandResponse>> CorporateRegister(CorporateRegisterCommandRequest request, CancellationToken cancellationToken)
        {
            // 1. Email unique kontrolü (EN BAŞTA)
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new ApiResponse<CorporateRegisterCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Bu e-posta adresi sistemde zaten kayıtlı. Lütfen giriş yapınız."
                };
            }

            // 2. UserTypes validasyonu
            var validUserTypes = new[] { "Emlakçı", "İnşaat", "Banka", "Vakıf", "Bireysel" };
            if (string.IsNullOrWhiteSpace(request.UserTypes) || !validUserTypes.Contains(request.UserTypes))
            {
                return new ApiResponse<CorporateRegisterCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Geçersiz kullanıcı tipi. Emlakçı, İnşaat, Banka, Vakıf veya Bireysel olmalıdır."
                };
            }

            // 3. Dosya kontrolü
            if (request.Level5Certificate == null || request.Level5Certificate.Length == 0)
            {
                return new ApiResponse<CorporateRegisterCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Belge dosyası zorunludur."
                };
            }

            if (request.TaxPlate == null || request.TaxPlate.Length == 0)
            {
                return new ApiResponse<CorporateRegisterCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Vergi Levhası zorunludur."
                };
            }

            var TaxPlate = Path.GetFileName(request.TaxPlate.FileName);

            var level5Cert = Path.GetFileName(request.Level5Certificate.FileName);

           // var uploadsDirectory = Path.Combine(GeneralConstants.WwwRootPath, "uploads", "estate_docs");
            var uploadsDirectory = Path.Combine(GeneralConstants.WwwRootPath, "uploads", "estate_docs");

            if (!Directory.Exists(uploadsDirectory))
                Directory.CreateDirectory(uploadsDirectory);

            var level5CertPath = Path.Combine(uploadsDirectory, level5Cert);

            var TaxPlatePath = Path.Combine(uploadsDirectory, TaxPlate);

            using (var stream = new FileStream(TaxPlatePath, FileMode.Create))
            {
                await request.TaxPlate.CopyToAsync(stream);
            }

            using (var stream = new FileStream(level5CertPath, FileMode.Create))
            {
                await request.Level5Certificate.CopyToAsync(stream);
            }

            // Use new flexible document fields
            request.Document1Path = GeneralConstants.BaseUrl + "/uploads/estate_docs/" + level5Cert;
            request.Document2Path = GeneralConstants.BaseUrl + "/uploads/estate_docs/" + TaxPlate;

            // Keep old fields for backward compatibility
            request.Level5CertificatePath = request.Document1Path;
            request.TaxPlatePath = request.Document2Path;

            if (request.ProfilePicture != null)
            {
                var profilePicture = Path.GetFileName(request.ProfilePicture.FileName);

                var profilePicturePath = Path.Combine(uploadsDirectory, profilePicture);

                using (var stream = new FileStream(profilePicturePath, FileMode.Create))
                {
                    await request.ProfilePicture.CopyToAsync(stream);
                }

                request.ProfilePicturePath = GeneralConstants.BaseUrl + "/uploads/estate_docs/" + profilePicture;
            }


            var user = _mapper.Map<ApplicationUser>(request);

            user.IsActive = "passive";

            user.RegistrationDate = DateTime.Now;

            // ✅ Otomatik Firma No Ata (Tip bazlı)
            user.UserOrder = await GetNextUserOrder(request.UserTypes);

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, request.UserTypes);

                    return new ApiResponse<CorporateRegisterCommandResponse>
                    {
                        Code = (int)HttpStatusCode.OK,
                        Data = _mapper.Map<CorporateRegisterCommandResponse>(user),
                        IsSuccessfull = true,
                        Message = "Ekleme İşlemi Başarılı"
                    };
                }

                if (result.Errors.Any(a => a.Code == "DuplicateUserName" || a.Code == "DuplicateEmail"))
                    return new ApiResponse<CorporateRegisterCommandResponse>
                    {
                        Code = 400,
                        Data = null,
                        IsSuccessfull = false,
                        Message = " Aynı e-posta adresi ile kayıt olunduğu için tekrar kayıt oluşturulamıyor!"
                    };


                return new ApiResponse<CorporateRegisterCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = string.Join(", ", result.Errors.Select(a => a.Description).ToList())
                };
            }
            catch (Exception ex)
            {
                // TODO: Add proper logging here (ILogger<UserService>)
                // Console.WriteLine($"Corporate registration error: {ex.Message}");
                return new ApiResponse<CorporateRegisterCommandResponse>
                {
                    Code = 500,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyiniz."
                };
            }
        }

        // Corporate Update Method
        public async Task<ApiResponse<CorporateUpdateCommandResponse>> CorporateUpdate(CorporateUpdateCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the existing user
                var user = await _readRepository.GetAsync(x => x.Id == request.Id, EnableTracking: true);

                if (user is null)
                {
                    return new ApiResponse<CorporateUpdateCommandResponse>
                    {
                        Code = 404,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Kullanıcı bulunamadı."
                    };
                }

                user = _mapper.Map(request, user);

                var uploadsDirectory = Path.Combine(GeneralConstants.WwwRootPath, "uploads", "estate_docs");
                if (!Directory.Exists(uploadsDirectory))
                    Directory.CreateDirectory(uploadsDirectory);


                // Handle profile picture update
                if (request.RemoveProfilePicture)
                {
                    // Remove existing profile picture
                    if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                    {
                        var oldProfilePath = user.ProfilePicturePath.Replace(GeneralConstants.BaseUrl + "/uploads/estate_docs/", "");
                        var oldProfileFilePath = Path.Combine(uploadsDirectory, oldProfilePath);
                        if (File.Exists(oldProfilePath) && user.ProfilePicturePath != "/Images/Common/defaultUser.png")
                        {
                            File.Delete(oldProfileFilePath);
                        }
                    }
                    user.ProfilePicturePath = null;
                }
                else if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
                {
                    // Delete old profile picture if exists
                    if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                    {
                        var oldProfilePath = user.ProfilePicturePath.Replace(GeneralConstants.BaseUrl + "/uploads/estate_docs/", "");
                        var oldProfileFilePath = Path.Combine(uploadsDirectory, oldProfilePath);
                        if (File.Exists(oldProfilePath) && user.ProfilePicturePath != "/Images/Common/defaultUser.png")
                        {
                            File.Delete(oldProfileFilePath);
                        }
                    }

                    // Upload new profile picture
                    var profilePicture = Path.GetFileName(request.ProfilePicture.FileName);

                    var profilePicturePath = Path.Combine(uploadsDirectory, profilePicture);

                    using (var stream = new FileStream(profilePicturePath, FileMode.Create))
                    {
                        await request.ProfilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePicturePath = GeneralConstants.BaseUrl + "/uploads/estate_docs/" + profilePicture;
                }



                if (request.RemoveLevel5Certificate)
                {
                    if (!string.IsNullOrEmpty(user.Level5CertificatePath))
                    {
                        var oldLevel5CertificatePath = user.Level5CertificatePath.Replace(GeneralConstants.BaseUrl + "/uploads/estate_docs/", "");
                        var oldLevel5CertificateFilePath = Path.Combine(uploadsDirectory, oldLevel5CertificatePath);
                        if (File.Exists(oldLevel5CertificatePath))
                        {
                            File.Delete(oldLevel5CertificateFilePath);
                        }
                    }
                    user.Level5CertificatePath = null;
                }
                else if (request.Level5Certificate != null && request.Level5Certificate.Length > 0)
                {
                    if (!string.IsNullOrEmpty(user.Level5CertificatePath))
                    {
                        var oldLevel5CertificatePath = user.Level5CertificatePath.Replace(GeneralConstants.BaseUrl + "/uploads/estate_docs/", "");
                        var oldLevel5CertificateFilePath = Path.Combine(uploadsDirectory, oldLevel5CertificatePath);
                        if (File.Exists(oldLevel5CertificatePath))
                        {
                            File.Delete(oldLevel5CertificateFilePath);
                        }
                    }

                    var level5Certificate = Path.GetFileName(request.Level5Certificate.FileName);
                    var level5CertificatePath = Path.Combine(uploadsDirectory, level5Certificate);

                    using (var stream = new FileStream(level5CertificatePath, FileMode.Create))
                    {
                        await request.Level5Certificate.CopyToAsync(stream);
                    }

                    user.Level5CertificatePath = GeneralConstants.BaseUrl + "/uploads/estate_docs/" + level5Certificate;
                }


                if (request.RemoveTaxPlate)
                {
                    if (!string.IsNullOrEmpty(user.TaxPlatePath))
                    {
                        var oldTaxPlatePath = user.TaxPlatePath.Replace(GeneralConstants.BaseUrl + "/uploads/estate_docs/", "");
                        var oldTaxPlateFilePath = Path.Combine(uploadsDirectory, oldTaxPlatePath);
                        if (File.Exists(oldTaxPlatePath))
                        {
                            File.Delete(oldTaxPlateFilePath);
                        }
                    }
                    user.TaxPlatePath = null;
                }
                else if (request.TaxPlate != null && request.TaxPlate.Length > 0)
                {
                    if (!string.IsNullOrEmpty(user.TaxPlatePath))
                    {
                        var oldTaxPlatePath = user.TaxPlatePath.Replace(GeneralConstants.BaseUrl + "/uploads/estate_docs/", "");
                        var oldTaxPlateFilePath = Path.Combine(uploadsDirectory, oldTaxPlatePath);
                        if (File.Exists(oldTaxPlatePath))
                        {
                            File.Delete(oldTaxPlateFilePath);
                        }
                    }

                    var taxPlate = Path.GetFileName(request.TaxPlate.FileName);
                    var taxPlatePath = Path.Combine(uploadsDirectory, taxPlate);

                    using (var stream = new FileStream(taxPlatePath, FileMode.Create))
                    {
                        await request.TaxPlate.CopyToAsync(stream);
                    }

                    user.TaxPlatePath = GeneralConstants.BaseUrl + "/uploads/estate_docs/" + taxPlate;
                }


                // Update password if provided
                if (!string.IsNullOrEmpty(request.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);

                    if (!passwordResult.Succeeded)
                    {
                        return new ApiResponse<CorporateUpdateCommandResponse>
                        {
                            Code = 400,
                            Data = null,
                            IsSuccessfull = false,
                            Message = "Şifre güncellenirken hata oluştu: " + string.Join(", ", passwordResult.Errors.Select(e => e.Description))
                        };
                    }
                }



                // Update the user
                var result = await _writeRepository.UpdateAsync(user);

                if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
                {
                    return new ApiResponse<CorporateUpdateCommandResponse>
                    {
                        Code = 200,
                        Data = new CorporateUpdateCommandResponse
                        {
                            Id = user.Id
                        },
                        IsSuccessfull = true,
                        Message = "Kurumsal kullanıcı başarıyla güncellendi."
                    };
                }

                return new ApiResponse<CorporateUpdateCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kurumsal kullanıcı güncellenemedi."
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<CorporateUpdateCommandResponse>
                {
                    Code = 500,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Güncelleme sırasında bir hata oluştu: " + e.Message
                };
            }
        }

        public async Task<ApiResponse<AddUserCommandResponse>> AddUser(AddUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<ApplicationUser>(request);
            var result = await _userManager.CreateAsync(user, request.Password);



            if (result.Succeeded)
            {
                var response = await CreateUserRolesAsync(user.Email, request.UserType);
                return new ApiResponse<AddUserCommandResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = _mapper.Map<AddUserCommandResponse>(user),
                    IsSuccessfull = true,
                    Message = "Ekleme İşlemi Başarılı"
                };
            }

            return new ApiResponse<AddUserCommandResponse>
            {
                Code = 400,
                Data = null,
                IsSuccessfull = false,
                Message = string.Join(", ", result.Errors)
            };

        }

        public async Task<ApiResponse<DeleteUserCommandResponse>> DeleteUser(DeleteUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _readRepository.GetAsync(x => x.Id == request.Id);
            if (user is null)
                return new ApiResponse<DeleteUserCommandResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kayıt Bulunamadı"
                };

            // Onay metni kontrolü (mobil ekip isteği)
            if (request.ConfirmationText != "HESABIMI SIL")
            {
                return new ApiResponse<DeleteUserCommandResponse>
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Onay metni yanlış. Tam olarak 'HESABIMI SIL' yazmalısınız."
                };
            }

            // Şifre kontrolü (mobil ekip isteği)
            if (!string.IsNullOrEmpty(request.Password))
            {
                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();
                var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                
                if (verificationResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                {
                    return new ApiResponse<DeleteUserCommandResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Şifre hatalı."
                    };
                }
            }

            // Soft delete: Kullanıcıyı işaretle
            user.IsActive = "deleted"; // IsActive ile soft delete simulation
            
            // Generate recovery token
            var recoveryToken = Guid.NewGuid().ToString("N");
            
            // Track deletion date for 30-day recovery window
            var deletionDate = DateTime.UtcNow;
            var recoveryDeadline = deletionDate.AddDays(30);
            
            var deletedAccount = new Sevval.Domain.Entities.DeletedAccount
            {
                UserId = user.Id,
                DeletedAt = deletionDate,
                DeletionReason = request.DeletionReason,
                RecoveryToken = recoveryToken
            };
            await _context.DeletedAccounts.AddAsync(deletedAccount, cancellationToken);
            
            // İlgili verileri cascade soft delete yap
            await SoftDeleteUserRelatedDataAsync(user.Id, user.Email, cancellationToken);

            await _writeRepository.UpdateAsync(user);

            if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
            {
                // Email bildirimini asenkron gönder (başarısızlık işlemi engellemez)
                try
                {
                    await _eMailService.SendAccountDeletionEmailAsync(new SendAccountDeletionDto
                    {
                        ReceiverEmail = user.Email,
                        ReceiverName = $"{user.FirstName} {user.LastName}",
                        DeletionDate = DateTime.UtcNow,
                        RecoveryDeadline = DateTime.UtcNow.AddDays(30),
                        RecoveryToken = recoveryToken,
                        UserId = user.Id
                    });
                }
                catch (Exception ex)
                {
                    // Email gönderimi başarısız olsa bile hesap silme devam etmeli
                    // Sadece log at
                    Console.WriteLine($"Email gönderimi başarısız: {ex.Message}");
                }

                return new ApiResponse<DeleteUserCommandResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = new DeleteUserCommandResponse
                    {
                        UserId = user.Id,
                        DeletedAt = deletionDate,
                        RecoveryDeadline = recoveryDeadline,
                        RecoveryToken = recoveryToken
                    },
                    IsSuccessfull = true,
                    Message = "Hesabınız başarıyla silindi. 30 gün içinde destek ekibimize başvurarak hesabınızı kurtarabilirsiniz."
                };
            }

            return new ApiResponse<DeleteUserCommandResponse>
            {
                Code = 400,
                Data = null,
                IsSuccessfull = false,
                Message = "Hesap silme işlemi başarısız oldu. Lütfen daha sonra tekrar deneyin."
            };

        }

        /// <summary>
        /// Kullanıcıya ait ilgili verileri cascade soft delete yapar
        /// </summary>
        private async Task SoftDeleteUserRelatedDataAsync(string userId, string userEmail, CancellationToken cancellationToken)
        {
            try
            {
                // IlanBilgileri - Kullanıcının ilanlarını "deleted" status yap
                var ilanlar = await _context.IlanBilgileri
                    .Where(x => x.Email == userEmail)
                    .ToListAsync(cancellationToken);

                foreach (var ilan in ilanlar)
                {
                    ilan.Status = "deleted"; // IlanModel'de de IsDeleted yok, Status var
                }

                // NOT: Diğer entity'lerde soft delete property'si bulunmuyor
                // Bu yüzden sadece IlanBilgileri için Status="deleted" pattern uygulanıyor

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Cascade delete hatası işlemi durdurmamalı
                Console.WriteLine($"İlgili verilerin silinmesi sırasında hata: {ex.Message}");
            }
        }

        public async Task<ApiResponse<GetUserByIdQueryResponse>> GetUser(GetUserByIdQueryRequest request)
        {
            var user = await _readRepository.GetAsync(x => x.Id == request.Id);

            if (user is null)
                return new ApiResponse<GetUserByIdQueryResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = new GetUserByIdQueryResponse(),
                    IsSuccessfull = false,
                    Message = "Kayıt Bulunamadı"
                };
            return new ApiResponse<GetUserByIdQueryResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = _mapper.Map<GetUserByIdQueryResponse>(user),
                IsSuccessfull = true,
                Message = "Kayıt Getirildi"
            };
        }

        public async Task<ApiResponse<UpdateUserCommandResponse>> UpdateUser(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {


            var user = await _readRepository.GetAsync(x => x.Id == request.Id, EnableTracking: true);

            user = _mapper.Map(request, user);




            var result = await _userManager.UpdateAsync(user);



            if (result.Succeeded)
            {
                return new ApiResponse<UpdateUserCommandResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = _mapper.Map<UpdateUserCommandResponse>(user),
                    IsSuccessfull = true,
                    Message = "Güncelleme İşlemi Başarılı"
                };
            }

            return new ApiResponse<UpdateUserCommandResponse>
            {
                Code = 400,
                Data = null,
                IsSuccessfull = false,
                Message = string.Join(", ", result.Errors)
            };
        }

        public async Task<ApiResponse<UpdateUserProfileCommandResponse>> UpdateUserProfile(UpdateUserProfileCommandRequest request, CancellationToken cancellationToken)
        {


            var user = await _readRepository.GetAsync(x => x.Id == request.Id, EnableTracking: true);

            user = _mapper.Map(request, user);



            var result = await _userManager.UpdateAsync(user);



            if (result.Succeeded)
            {
                return new ApiResponse<UpdateUserProfileCommandResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = _mapper.Map<UpdateUserProfileCommandResponse>(user),
                    IsSuccessfull = true,
                    Message = "Güncelleme İşlemi Başarılı"
                };
            }

            return new ApiResponse<UpdateUserProfileCommandResponse>
            {
                Code = 400,
                Data = null,
                IsSuccessfull = false,
                Message = string.Join(", ", result.Errors)
            };
        }


        public async Task SeedDefaultRoles()
        {
            var defaultRoles = new[]
            {
            "Bireysel",
            "Emlakçı",   // 🆕 Yeni kurumsal kayıt sistemi
            "Kurumsal",  // ⚠️ Geriye dönük uyumluluk için
            "İnşaat",
            "Vakıf",
            "Banka"
        };

            foreach (var role in defaultRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new Role { Name = role });
                }
            }
        }

        private async Task<bool> CreateUserRolesAsync(string email, string rolename)
        {


            try
            {

                var user = await _userManager.FindByEmailAsync(email);

                await _userManager.AddToRoleAsync(user, rolename);
            }
            catch (Exception s)
            {

                return false;
            }


            return true;
        }

        public async Task<ApiResponse<CheckUserExistsQueryResponse>> CheckUserExists(CheckUserExistsQueryRequest request)
        {
            var user = await _readRepository.GetAsync(x => x.Email == request.Email || x.PhoneNumber == request.Phone);

            if (user is null)
                return new ApiResponse<CheckUserExistsQueryResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = new CheckUserExistsQueryResponse() { Exists = false },
                    IsSuccessfull = true,
                    Message = "Kayıt Bulunamadı"
                };

            return new ApiResponse<CheckUserExistsQueryResponse>
            {
                Code = (int)HttpStatusCode.OK,
                Data = new CheckUserExistsQueryResponse() { Exists = true },
                IsSuccessfull = true,
            };
        }

        public async Task<ApiResponse<bool>> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception)
            {

                return new ApiResponse<bool>
                {
                    IsSuccessfull = false,
                };
            }

            return new ApiResponse<bool>
            {
                IsSuccessfull = true,
            };

        }


        public async Task<ApiResponse<ForgottenPasswordCommandResponse>> ForgotPassword(ForgottenPasswordCommandRequest request)
        {

            var user = await _userManager.FindByEmailAsync(request.Email);


            var token = await _readUserRefreshTokenRepository.AnyAsync(a => a.UserId == user.Id && a.Expiration > DateTime.Now);

            if (!token)
            {
                return new ApiResponse<ForgottenPasswordCommandResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = new ForgottenPasswordCommandResponse(),
                    IsSuccessfull = false,
                    Message = "Şifre yenileme token süreniz dolmuştur."
                };
            }

            if (user is null)
                return new ApiResponse<ForgottenPasswordCommandResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = new ForgottenPasswordCommandResponse(),
                    IsSuccessfull = false,
                    Message = "Kayıt Bulunamadı"
                };

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

            if (result.Succeeded)
                return new ApiResponse<ForgottenPasswordCommandResponse>
                {
                    Message = "Şifre güncellendi",
                    IsSuccessfull = true,
                    Data = new ForgottenPasswordCommandResponse(),
                    Code = (int)HttpStatusCode.OK,

                };


            return new ApiResponse<ForgottenPasswordCommandResponse>
            {
                Code = (int)HttpStatusCode.BadRequest,
                Data = new ForgottenPasswordCommandResponse(),
                IsSuccessfull = false,
                Message = "İşlem gerçekleştirilemedi."
            };

        }

        public async Task<ApiResponse<SendNewCodeCommandResponse>> SendNewCode(SendNewCodeCommandRequest request, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return new ApiResponse<SendNewCodeCommandResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = new SendNewCodeCommandResponse(),
                    IsSuccessfull = false,
                    Message = "Kayıt Bulunamadı"
                };


            string uniqueData = $"{request.Email}-{DateTime.UtcNow.Ticks}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GeneralConstants.SecretKey));

            string verificationCode = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(uniqueData)))
                .Replace("-", "")
                .Substring(0, Math.Min(5, 32));

            await _writeUserRefreshTokenRepository.AddAsync(new UserRefreshToken()
            {
                Expiration = DateTime.Now.AddDays(1),
                Code = verificationCode,
                UserId = user.Id
            });

            if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
            {
                var response = await _emailService.SendPasswordResetEmailAsync(new Application.Dtos.Email.ForgotPasswordViewDto
                {
                    Email = request.Email,
                    Code = verificationCode,
                    NewPassword = ""
                }, verificationCode);

                if (response)
                {
                    return new ApiResponse<SendNewCodeCommandResponse>
                    {
                        Code = 200,
                        Data = new SendNewCodeCommandResponse { Successfull = true },
                        IsSuccessfull = true,
                        Message = "Şifre sıfırlama maili gönderildi."
                    };
                }
            }


            return new ApiResponse<SendNewCodeCommandResponse>
            {
                Code = 400,
                Data = null,
                IsSuccessfull = false,
                Message = "İşlem sırasında hata oluştu"
            };
        }

        public async Task<ApiResponse<ConfirmEstateCommandResponse>> ConfirmEstate(ConfirmEstateCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _readRepository.GetAsync(x => x.Email == request.Email);

            var maxuser = _readRepository.Queryable().Max(x => x.UserOrder); ;

            if (user is null)
                return new ApiResponse<ConfirmEstateCommandResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = new ConfirmEstateCommandResponse(),
                    IsSuccessfull = false,
                    Message = "Kullanıcı Bulunamadı"
                };

            user.EmailConfirmed = true;

            user.IsActive = "active";

            user.UserOrder = maxuser + 1;

            await _writeRepository.UpdateAsync(user);

            if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
            {
                return new ApiResponse<ConfirmEstateCommandResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = new ConfirmEstateCommandResponse { IsSuccessful = true },
                    IsSuccessfull = true,
                    Message = "Kullanıcı onaylandı ve aktif edildi.",

                };
            }

            return new ApiResponse<ConfirmEstateCommandResponse>
            {
                Code = (int)HttpStatusCode.NotFound,
                Data = new ConfirmEstateCommandResponse(),
                IsSuccessfull = false,
                Message = "İşlem sırasında hata meydana geldi"
            };

        }


        public async Task<ApiResponse<RejectEstateCommandResponse>> RejectEstate(RejectEstateCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _readRepository.GetAsync(x => x.Email == request.Email);

            var maxuser = _readRepository.Queryable().Max(x => x.UserOrder); ;

            if (user is null)
                return new ApiResponse<RejectEstateCommandResponse>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = new RejectEstateCommandResponse(),
                    IsSuccessfull = false,
                    Message = "Kullanıcı Bulunamadı"
                };




            await _writeRepository.DeleteAsync(user);

            if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
            {
                return new ApiResponse<RejectEstateCommandResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = new RejectEstateCommandResponse { IsSuccessful = true },
                    IsSuccessfull = true,
                    Message = "Kullanıcı reddedildi.",

                };
            }

            return new ApiResponse<RejectEstateCommandResponse>
            {
                Code = (int)HttpStatusCode.NotFound,
                Data = new RejectEstateCommandResponse(),
                IsSuccessfull = false,
                Message = "İşlem sırasında hata meydana geldi"
            };

        }

        public async Task<ApiResponse<LoginWithSocialMediaCommandResponse>> LoginWithSocialMedia(LoginWithSocialMediaCommandRequest request, CancellationToken cancellationToken)
        {

            var user = _mapper.Map<ApplicationUser>(request);


            try
            {

                var existingUser = await _userManager.FindByLoginAsync(request.Provider, request.SocialId);

                var existuser = await _readRepository.GetAsync(a => a.Email == user.Email, EnableTracking: true);


                if (existingUser is null && existuser is null && request.UserType == "Bireysel")
                {

                    user.UserTypes = request.UserType;
                    user.IsSubscribed = "ücretsiz";
                    user.IsActive = "active";
                    user.UserOrder = 1;
                    user.RegistrationDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(request.PhotoUrl))
                    {


                        using var httpClient = new HttpClient();
                        byte[] imageBytes = await httpClient.GetByteArrayAsync(request.PhotoUrl);


                        var fileName = Guid.NewGuid().ToString() + ".jpg";
                        var uploadsDirectory = Path.Combine(GeneralConstants.WwwRootPath, "Images", "UserImages");

                        // Uploads klasörü yoksa oluştur
                        if (!Directory.Exists(uploadsDirectory))
                        {
                            Directory.CreateDirectory(uploadsDirectory);
                        }

                        if (imageBytes != null)
                        {
                            string bytesPath = Path.Combine(uploadsDirectory, fileName);
                            await File.WriteAllBytesAsync(bytesPath, imageBytes);
                        }

                        // Yüklenen fotoğrafın yolunu kaydet
                        user.ProfilePicturePath = "/Images/UserImages/" + fileName;
                    }
                    else
                    {
                        // Kullanıcı fotoğrafı yüklemediyse varsayılan yolu ata
                        user.ProfilePicturePath = "/Images/Common/defaultUser.png";
                    }

                    var result = await _userManager.CreateAsync(user);

                    if (result.Succeeded)
                    {

                        var result2 = await _userManager.AddLoginAsync(user, new UserLoginInfo(request.Provider, request.SocialId, null));

                        await _userManager.AddToRoleAsync(user, request.UserType);

                        if (!result2.Succeeded)
                        {
                            return new ApiResponse<LoginWithSocialMediaCommandResponse>
                            {
                                Code = 400,
                                Data = null,
                                IsSuccessfull = false,
                                Message = result2.Errors?.Select(a => a.Description).FirstOrDefault()
                            };
                        }

                    }
                    else
                    {
                        return new ApiResponse<LoginWithSocialMediaCommandResponse>
                        {
                            Code = 400,
                            Data = null,
                            IsSuccessfull = false,
                            Message = result.Errors?.Select(a => a.Description).FirstOrDefault()
                        };
                    }

                    if (result.Errors.Any(a => a.Code == "DuplicateUserName" || a.Code == "DuplicateEmail"))
                        return new ApiResponse<LoginWithSocialMediaCommandResponse>
                        {
                            Code = 400,
                            Data = null,
                            IsSuccessfull = false,
                            Message = "Aynı e-posta adresi ile kayıt olunduğu için tekrar kayıt oluşturulamıyor!"
                        };
                }
                else if (existingUser is null && existuser is not null)
                {

                    var result2 = await _userManager.AddLoginAsync(existuser, new UserLoginInfo(request.Provider, request.SocialId, null));

                    if (!result2.Succeeded)
                    {
                        return new ApiResponse<LoginWithSocialMediaCommandResponse>
                        {
                            IsSuccessfull = false,
                            Message = "Giriş yapılamadı. Normal giriş yapmayı deneyiniz.",
                            Code = 400,
                        };
                    }
                    user = existuser;
                }
                else if (existingUser is not null && existuser is not null)
                {
                    user = existuser;
                }


                if (user.IsActive != "active")
                {
                    return new ApiResponse<LoginWithSocialMediaCommandResponse>
                    {
                        IsSuccessfull = false,
                        Message = "Kullanıcı bulunamadı, lütfen kayıt olunuz.",
                        Code = 400
                    };
                }

                var tokenResponse = await _tokenService.CreateToken(user);

                var userRefreshToken = await _readUserRefreshTokenRepository.GetAsync(a => a.UserId == user.Id);

                if (userRefreshToken is null)
                {
                    await _writeUserRefreshTokenRepository.AddAsync(new UserRefreshToken
                    {
                        UserId = user.Id,
                        Code = tokenResponse.Data.RefreshToken,
                        Expiration = tokenResponse.Data.RefreshTokenExpiration,

                    });
                }
                else
                {
                    userRefreshToken.Code = tokenResponse.Data.RefreshToken;
                    userRefreshToken.Expiration = tokenResponse.Data.RefreshTokenExpiration;
                    await _writeUserRefreshTokenRepository.UpdateAsync(userRefreshToken);
                }

                if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
                {
                    return new ApiResponse<LoginWithSocialMediaCommandResponse>
                    {
                        Data = new LoginWithSocialMediaCommandResponse
                        {
                            AccessToken = tokenResponse.Data.AccessToken,
                            RefreshToken = tokenResponse.Data.RefreshToken,
                            IsSuccessful = true,
                            Message = "Oturum açıldı",
                            AccessTokenExpiration = tokenResponse.Data.AccessTokenExpiration,
                            RefreshTokenExpiration = tokenResponse.Data.RefreshTokenExpiration,
                        },
                        Code = 200,
                        IsSuccessfull = true,
                        Message = "Oturum Başarılı"
                    };
                }



                return new ApiResponse<LoginWithSocialMediaCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = tokenResponse.Message
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<LoginWithSocialMediaCommandResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Bir hata oluştu"
                };

            }

        }

        public async Task<ApiResponse<RefreshTokenCommandResponse>> RefreshToken(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Find the refresh token in the database
                var existingRefreshToken = await _readUserRefreshTokenRepository
                    .GetAsync(rt => rt.Code == request.RefreshToken);

                if (existingRefreshToken is null)
                {
                    return new ApiResponse<RefreshTokenCommandResponse>
                    {
                        Code = 401,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Geçersiz refresh token"
                    };
                }

                // Check if refresh token is expired
                if (existingRefreshToken.Expiration <= DateTime.Now)
                {
                    // Remove expired token
                    await _writeUserRefreshTokenRepository.DeleteAsync(existingRefreshToken);

                    if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
                    {
                        return new ApiResponse<RefreshTokenCommandResponse>
                        {
                            Code = 401,
                            Data = null,
                            IsSuccessfull = false,
                            Message = "Refresh token süresi dolmuş"
                        };
                    }


                }

                // Get the user associated with the refresh token
                var user = await _userManager.FindByIdAsync(existingRefreshToken.UserId);
                if (user == null)
                {
                    return new ApiResponse<RefreshTokenCommandResponse>
                    {
                        Code = 404,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Kullanıcı bulunamadı"
                    };
                }

                // Generate new tokens
                var tokenResponse = await _tokenService.CreateToken(user);
                if (!tokenResponse.IsSuccessfull)
                {
                    return new ApiResponse<RefreshTokenCommandResponse>
                    {
                        Code = 500,
                        Data = null,
                        IsSuccessfull = false,
                        Message = "Token oluşturulamadı"
                    };
                }

                // Remove the old refresh token
                await _writeUserRefreshTokenRepository.DeleteAsync(existingRefreshToken);

                // Save the new refresh token
                var newRefreshToken = new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = tokenResponse.Data.RefreshToken,
                    Expiration = tokenResponse.Data.RefreshTokenExpiration
                };

                await _writeUserRefreshTokenRepository.AddAsync(newRefreshToken);


                if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
                {
                    // Return the new tokens
                    var response = new RefreshTokenCommandResponse
                    {
                        AccessToken = tokenResponse.Data.AccessToken,
                        AccessTokenExpiration = tokenResponse.Data.AccessTokenExpiration,
                        RefreshToken = tokenResponse.Data.RefreshToken,
                        RefreshTokenExpiration = tokenResponse.Data.RefreshTokenExpiration
                    };

                    return new ApiResponse<RefreshTokenCommandResponse>
                    {
                        Code = 200,
                        Data = response,
                        IsSuccessfull = true,
                        Message = "Token yenilendi"
                    };
                }



            }
            catch (Exception ex)
            {
                return new ApiResponse<RefreshTokenCommandResponse>
                {
                    Code = 500,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Token yenileme sırasında bir hata oluştu: " + ex.Message
                };
            }

            return new ApiResponse<RefreshTokenCommandResponse>
            {
                Code = 500,
                Data = null,
                IsSuccessfull = false,
                Message = "Token yenileme sırasında bir hata oluştu"
            };
        }

        public async Task<ApiResponse<UpdatePasswordCommandResponse>> UpdatePassword(UpdatePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Find user by ID
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new ApiResponse<UpdatePasswordCommandResponse>
                    {
                        Code = (int)HttpStatusCode.NotFound,
                        Data = new UpdatePasswordCommandResponse { IsSuccessful = false },
                        IsSuccessfull = false,
                        Message = "Kullanıcı bulunamadı."
                    };
                }

                // Verify current password
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return new ApiResponse<UpdatePasswordCommandResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Data = new UpdatePasswordCommandResponse { IsSuccessful = false },
                        IsSuccessfull = false,
                        Message = "Mevcut şifre yanlış."
                    };
                }

                // Update password
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (result.Succeeded)
                {
                    return new ApiResponse<UpdatePasswordCommandResponse>
                    {
                        Code = (int)HttpStatusCode.OK,
                        Data = new UpdatePasswordCommandResponse { IsSuccessful = true, Message = "Şifre başarıyla güncellendi." },
                        IsSuccessfull = true,
                        Message = "Şifre başarıyla güncellendi."
                    };
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ApiResponse<UpdatePasswordCommandResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Data = new UpdatePasswordCommandResponse { IsSuccessful = false },
                        IsSuccessfull = false,
                        Message = $"Şifre güncellenirken hata oluştu: {errors}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<UpdatePasswordCommandResponse>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Data = new UpdatePasswordCommandResponse { IsSuccessful = false },
                    IsSuccessfull = false,
                    Message = "Şifre güncellenirken beklenmeyen bir hata oluştu."
                };
            }
        }

        /// <summary>
        /// Verilen kullanıcı tipine göre bir sonraki UserOrder numarasını hesaplar
        /// </summary>
        private async Task<int> GetNextUserOrder(string userType)
        {
            // Aynı tip için son kullanıcıyı bul
            var lastUser = await _context.Users
                .Where(u => u.UserTypes == userType && u.UserOrder > 0)
                .OrderByDescending(u => u.UserOrder)
                .FirstOrDefaultAsync();

            // Son numara + 1 döndür, yoksa 1'den başla
            return (lastUser?.UserOrder ?? 0) + 1;
        }

    }



}

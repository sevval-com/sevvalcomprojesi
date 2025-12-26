using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Sevval.Application.Abstractions.Services;
using Sevval.Application.Dtos.SmtpSettings;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;
using Sevval.Domain.Entities;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Sevval.Infrastructure.Services;

public class InvestmentRequestService : IInvestmentRequestService
{
    private readonly IReadRepository<SatisTalep> _readRepository;
    private readonly IReadRepository<ApplicationUser> _readApplicationUserRepository;
    private readonly IWriteRepository<SatisTalep> _writeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InvestmentRequestService(IReadRepository<SatisTalep> readRepository, IWriteRepository<SatisTalep> writeRepository, IUnitOfWork unitOfWork, IReadRepository<ApplicationUser> readApplicationUserRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _readApplicationUserRepository = readApplicationUserRepository;
    }

    public async Task<ApiResponse<CreateInvestmentRequestCommandResponse>> CreateInvestmentRequestAsync(CreateInvestmentRequestCommandRequest request, CancellationToken cancellationToken)
    {
        var investmentRequest = new SatisTalep
        {
            selectCategory = request.SelectCategory,
            selectStatus = request.SelectStatus,
            Rooms = request.Rooms,
            Area = request.Area,
            BuildingAge = request.BuildingAge,
            Floor = request.Floor,
            Bathrooms = request.Bathrooms,
            HeatingSystem = request.HeatingSystem,
            Parking = request.Parking,
            Price = request.Price,
            ResidentialCity = request.ResidentialCity,
            ResidentialDistrict = request.ResidentialDistrict,
            ResidentialVillage = request.ResidentialVillage,
            LandAda = null,
            LandParsel = null,
            LandArea = request.LandArea,
            Slope = request.Slope,
            RoadCondition = request.RoadCondition,
            DistanceToSettlement = request.DistanceToSettlement,
            ZoningStatus = request.ZoningStatus,
            LandCity = request.LandCity,
            LandDistrict = request.LandDistrict,
            LandVillage = request.LandVillage,
            LandPrice = request.LandPrice,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            LivingCity = request.LivingCity,
            InterestedPerson = request.InterestedPerson,
            MinBudget = request.MinBudget,
            MaxBudget = request.MaxBudget,
            CreatedDate = DateTime.Now
        };

        await _writeRepository.AddAsync(investmentRequest);

        if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
        {
            await MailSend(request);

            return new ApiResponse<CreateInvestmentRequestCommandResponse>
            {
                Code = 200,
                IsSuccessfull = true,
                Message = "Yatırım talebi başarıyla oluşturuldu.",
                Data = new CreateInvestmentRequestCommandResponse
                {
                    IsSuccessful = true,
                    Message = "Yatırım talebi başarıyla oluşturuldu.",
                    CreatedDate = investmentRequest.CreatedDate
                }
            };
        }

        return new ApiResponse<CreateInvestmentRequestCommandResponse>
        {
            Code = 500,
            IsSuccessfull = false,
            Message = "Yatırım talebi oluşturulamadı.",
            Data = new CreateInvestmentRequestCommandResponse
            {
                IsSuccessful = false,
                Message = "Yatırım talebi oluşturulamadı."
            }
        };
    }

    private async Task MailSend(CreateInvestmentRequestCommandRequest request)
    {
        var smtpSettings = new SmtpSetting
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "sevvalsiteonay@gmail.com",
            Password = "ztqa ycdd ghsp grlc",
            FromAddress = "sevvalsiteonay@gmail.com",
            AdminAddress = "sftumen41@gmail.com",
            AdminAddress2 = "ceritahsin0@gmail.com"
        };

        try
        {
            // Şehir bilgisine göre kullanıcılardan mail adreslerini çek
            List<string> recipientEmails = new List<string>();
            string targetCity = !string.IsNullOrEmpty(request.ResidentialCity)
                ? request.ResidentialCity : request.LandCity;

            if (!string.IsNullOrEmpty(targetCity))
            {
                var usersToNotify = _readApplicationUserRepository.Queryable()
                    .Where(user => user.City == targetCity && (user.UserTypes == "Emlakçı" || user.UserTypes == "Kurumsal") &&
                    user.IsActive == "active")
                    .Select(user => user.Email)
                    .ToList();

                recipientEmails.AddRange(usersToNotify);
            }

            //TODO : canlıda silinecek
            //recipientEmails.Clear();
            //recipientEmails.Add("huseyinakdag38@gmail.com");

            // Eşleşen kullanıcı bulunamazsa, mail admin'e gönderilsin.
            if (recipientEmails.Count == 0)
            {
                using (var client = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

                    var message = new MailMessage();
                    message.From = new MailAddress(smtpSettings.FromAddress);
                    message.To.Add(smtpSettings.AdminAddress);
                    message.To.Add(smtpSettings.AdminAddress2);
                    message.Subject = "Yeni Yatırım Talebi";
                    message.Body = CreateInvestmentEmailBody(request);
                    message.IsBodyHtml = true;

                    await client.SendMailAsync(message);
                }
            }
            else
            {
                using (var client = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

                    foreach (var email in recipientEmails)
                    {
                        var message = new MailMessage();
                        message.From = new MailAddress(smtpSettings.FromAddress);
                        message.To.Add(email);
                        message.CC.Add(smtpSettings.AdminAddress);
                        message.CC.Add(smtpSettings.AdminAddress2);
                        message.Subject = "Yeni Yatırım Talebi";
                        message.Body = CreateInvestmentEmailBody(request);
                        message.IsBodyHtml = true;

                        await client.SendMailAsync(message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (you might want to use a logging framework)
            Console.WriteLine($"Email sending failed: {ex.Message}");
        }
    }

    private string CreateInvestmentEmailBody(CreateInvestmentRequestCommandRequest request)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html><body>");
        sb.AppendLine("<h2>Yeni Yatırım Talebi</h2>");
        sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");

        sb.AppendLine($"<tr><td><strong>Ad Soyad:</strong></td><td>{request.FirstName} {request.LastName}</td></tr>");
        sb.AppendLine($"<tr><td><strong>E-posta:</strong></td><td>{request.Email}</td></tr>");
        sb.AppendLine($"<tr><td><strong>Telefon:</strong></td><td>{request.Phone}</td></tr>");

        if (!string.IsNullOrEmpty(request.SelectCategory))
            sb.AppendLine($"<tr><td><strong>Kategori:</strong></td><td>{request.SelectCategory}</td></tr>");

        if (!string.IsNullOrEmpty(request.SelectStatus))
            sb.AppendLine($"<tr><td><strong>Durum:</strong></td><td>{request.SelectStatus}</td></tr>");

        if (!string.IsNullOrEmpty(request.MinBudget))
            sb.AppendLine($"<tr><td><strong>Minimum Bütçe:</strong></td><td>{request.MinBudget} TL</td></tr>");

        if (!string.IsNullOrEmpty(request.MaxBudget))
            sb.AppendLine($"<tr><td><strong>Maksimum Bütçe:</strong></td><td>{request.MaxBudget} TL</td></tr>");

        if (!string.IsNullOrEmpty(request.ResidentialCity))
            sb.AppendLine($"<tr><td><strong>Şehir:</strong></td><td>{request.ResidentialCity}</td></tr>");

        if (!string.IsNullOrEmpty(request.ResidentialDistrict))
            sb.AppendLine($"<tr><td><strong>İlçe:</strong></td><td>{request.ResidentialDistrict}</td></tr>");

        if (!string.IsNullOrEmpty(request.Area))
            sb.AppendLine($"<tr><td><strong>Alan:</strong></td><td>{request.Area} m²</td></tr>");

        if (!string.IsNullOrEmpty(request.Rooms))
            sb.AppendLine($"<tr><td><strong>Oda Sayısı:</strong></td><td>{request.Rooms}</td></tr>");

        if (!string.IsNullOrEmpty(request.BuildingAge))
            sb.AppendLine($"<tr><td><strong>Bina Yaşı:</strong></td><td>{request.BuildingAge}</td></tr>");

        if (!string.IsNullOrEmpty(request.LivingCity))
            sb.AppendLine($"<tr><td><strong>Yaşadığı Şehir:</strong></td><td>{request.LivingCity}</td></tr>");

        sb.AppendLine("</table>");
        sb.AppendLine("<br><p>Bu yatırım talebi sevval.com.tr üzerinden gönderilmiştir.</p>");
        sb.AppendLine("</body></html>");

        return sb.ToString();
    }
}

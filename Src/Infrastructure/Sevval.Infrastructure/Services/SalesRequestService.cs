using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Abstractions.Services;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using System.Net.Mail;
using System.Net;
using Sevval.Application.Dtos.SmtpSettings;
using Sevval.Application.Dtos.FormDatas;
using System.Drawing;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rectangle = iTextSharp.text.Rectangle;
using Document = iTextSharp.text.Document;

namespace Sevval.Infrastructure.Services;

public class SalesRequestService : ISalesRequestService
{
    private readonly IReadRepository<SatisTalep> _readRepository;
    private readonly IReadRepository<ApplicationUser> _readApplicationUserRepository;


    private readonly IWriteRepository<SatisTalep> _writeRepository;
    private readonly IUnitOfWork _unitOfWork;


    public SalesRequestService(IReadRepository<SatisTalep> readRepository, IWriteRepository<SatisTalep> writeRepository, IUnitOfWork unitOfWork, IReadRepository<ApplicationUser> readApplicationUserRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _readApplicationUserRepository = readApplicationUserRepository;
    }

    public async Task<ApiResponse<CreateSalesRequestCommandResponse>> CreateSalesRequestAsync(CreateSalesRequestCommandRequest request, CancellationToken cancellationToken)
    {
        var salesRequest = new SatisTalep
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
            LandAda = request.LandAda,
            LandParsel = request.LandParsel,
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
            CreatedDate = DateTime.Now
        };

        await _writeRepository.AddAsync(salesRequest);

        if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
        {

            await MailSend(request);

            return new ApiResponse<CreateSalesRequestCommandResponse>
            {
                Code = 200,
                IsSuccessfull = true,
                Message = "Satış talebi oluşturuldu.",
                Data = new CreateSalesRequestCommandResponse { IsSuccessful = true }
            };
        }

        return new ApiResponse<CreateSalesRequestCommandResponse>
        {
            Code = 500,
            IsSuccessfull = false,
            Message = "Satış talebi oluşturulamadı.",
            Data = new CreateSalesRequestCommandResponse { IsSuccessful = false }
        };


    }



    private async Task MailSend(CreateSalesRequestCommandRequest request)
    {
        var smtpSettings = new SmtpSetting
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            Username = "sevvalsiteonay@gmail.com",
            Password = "ztqa ycdd ghsp grlc",
            FromAddress = "sevvalsiteonay@gmail.com",
            AdminAddress = "sftumen41@gmail.com",
            AdminAddress2 = "ceritahsin0@gmail.com" // İkinci admin mail adresini ekledik
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
                    user.IsActive=="active")
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
                    client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings.FromAddress),
                        Subject = "Mülk Satış Talebi - Eşleşen Kullanıcı Bulunamadı",
                        Body = "Sistemde bu şehirle eşleşen kullanıcı bulunamadı. Form bilgileri ekte yer almaktadır.",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(smtpSettings.AdminAddress);
                    mailMessage.To.Add(smtpSettings.AdminAddress2); // İkinci admin adresini ekledik

                    // PDF oluştur ve mail ekine ekle
                    byte[] pdfBytes = GeneratePdf(request);
                    var attachment = new Attachment(new MemoryStream(pdfBytes), "mulk-bilgileri.pdf", "application/pdf");
                    mailMessage.Attachments.Add(attachment);

                    await client.SendMailAsync(mailMessage);
                }
               
            }

            // Alıcılara ve Admin'lere Mülk Satış Talebi maili gönder
            using (var client = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.SmtpPort))
            {
                client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings.FromAddress),
                    Subject = "Mülk Satış Talebi",
                    Body = BuildEmailBody(request),
                    IsBodyHtml = true
                };

                foreach (var email in recipientEmails)
                {
                    mailMessage.To.Add(email);
                }
                mailMessage.To.Add(smtpSettings.AdminAddress); // Admin adresine de ekle
                mailMessage.To.Add(smtpSettings.AdminAddress2); // İkinci admin adresine de ekle

                // PDF oluştur ve mail ekine ekle
                byte[] pdfBytes = GeneratePdf(request);
                var attachment = new Attachment(new MemoryStream(pdfBytes), "mulk-bilgileri.pdf", "application/pdf");
                mailMessage.Attachments.Add(attachment);

                await client.SendMailAsync(mailMessage);
            }

            // Talep sahibine bilgilendirme maili gönder
            using (var client = new SmtpClient(smtpSettings.SmtpServer, smtpSettings.SmtpPort))
            {
                client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                client.EnableSsl = true;

                var ownerMailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings.FromAddress),
                    Subject = "Mülk Satış Talebiniz Alınmıştır",
                    Body = BuildOwnerEmailBody(), // Talep sahibine özel mail içeriği
                    IsBodyHtml = true
                };
                ownerMailMessage.To.Add(request.Email);
                await client.SendMailAsync(ownerMailMessage);
            }

           
        }
        catch (Exception ex)
        {
            throw;
        }


    }

    private string BuildEmailBody(CreateSalesRequestCommandRequest formData)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset='utf-8'/>");
        sb.AppendLine("<style>");
        sb.AppendLine("body { font-family: 'Arial', sans-serif; background-color: #f4f8fb; margin: 0; padding: 0; }");
        sb.AppendLine(".container { width: 90%; max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 0 15px rgba(0,0,0,0.1); overflow: hidden; }");
        sb.AppendLine(".header { background: linear-gradient(to right, #003366, #004080); padding: 20px; text-align: center; }");
        sb.AppendLine(".header img { max-width: 80px; }");
        sb.AppendLine(".header h2 { color: #fff; margin: 10px 0 0 0; font-size: 24px; }");
        sb.AppendLine(".content { padding: 20px; }");
        sb.AppendLine(".section { margin-bottom: 20px; }");
        sb.AppendLine(".section h3 { color: #003366; border-bottom: 2px solid #003366; padding-bottom: 5px; margin-bottom: 10px; }");
        sb.AppendLine(".section table { width: 100%; border-collapse: collapse; }");
        sb.AppendLine(".section table th, .section table td { padding: 8px; text-align: left; border: 1px solid #ddd; }");
        sb.AppendLine(".section table th { background-color: #003366; color: #fff; }");
        sb.AppendLine(".footer { text-align: center; padding: 15px; background-color: #f0f0f0; color: #003366; font-size: 12px; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class='container'>");
        sb.AppendLine("<div class='header'>");
        sb.AppendLine("<img src='https://i.hizliresim.com/sw39o6d.png' alt='Şirket Logosu' />");
        sb.AppendLine("<h2>Yeni Mülk Satış Talebi</h2>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class='content'>");

        // Kategori Bilgileri
        sb.AppendLine(BuildSectionHtml("Kategori Bilgileri", new List<(string, string)>
            {
                ("Kategori", formData.SelectCategory),
                ("Durum", formData.SelectStatus)
            }));

        // Mülk Bilgileri
        if (!string.IsNullOrEmpty(formData.Rooms) || !string.IsNullOrEmpty(formData.Area) ||
            !string.IsNullOrEmpty(formData.BuildingAge) || !string.IsNullOrEmpty(formData.Floor) ||
            !string.IsNullOrEmpty(formData.Bathrooms) || !string.IsNullOrEmpty(formData.Price) ||
            !string.IsNullOrEmpty(formData.ResidentialCity) || !string.IsNullOrEmpty(formData.ResidentialDistrict) ||
            !string.IsNullOrEmpty(formData.ResidentialVillage))
        {
            sb.AppendLine(BuildSectionHtml("Mülk Bilgileri", new List<(string, string)>
                {
                    ("Oda Sayısı", formData.Rooms),
                    ("Metrekare", formData.Area),
                    ("Bina Yaşı", formData.BuildingAge),
                    ("Kat", formData.Floor),
                    ("Banyo Sayısı", formData.Bathrooms),
                    ("Fiyat", formData.Price),
                    ("Konum", $"{formData.ResidentialCity}, {formData.ResidentialDistrict}, {formData.ResidentialVillage}")
                }));
        }

        // Arazi Bilgileri
        if (!string.IsNullOrEmpty(formData.LandAda) || !string.IsNullOrEmpty(formData.LandParsel) ||
            !string.IsNullOrEmpty(formData.LandArea) || !string.IsNullOrEmpty(formData.Slope) ||
            !string.IsNullOrEmpty(formData.RoadCondition) || !string.IsNullOrEmpty(formData.DistanceToSettlement) ||
            !string.IsNullOrEmpty(formData.ZoningStatus) || !string.IsNullOrEmpty(formData.LandCity) ||
            !string.IsNullOrEmpty(formData.LandDistrict) || !string.IsNullOrEmpty(formData.LandVillage))
        {
            sb.AppendLine(BuildSectionHtml("Arazi Bilgileri", new List<(string, string)>
                {
                    ("Ada", formData.LandAda),
                    ("Parsel", formData.LandParsel),
                    ("Metrekare", formData.LandArea),
                    ("Meyil Durumu", formData.Slope),
                    ("Yol Durumu", formData.RoadCondition),
                    ("Yerleşim Uzaklığı", formData.DistanceToSettlement),
                    ("İmar Durumu", formData.ZoningStatus),
                    ("Fiyat", formData.LandPrice),
                    ("Konum", $"{formData.LandCity}, {formData.LandDistrict}, {formData.LandVillage}")
                }));
        }

        // Mülk Sahibi Bilgileri
        sb.AppendLine(BuildSectionHtml("Mülk Sahibi Bilgileri", new List<(string, string)>
            {
                ("Adı", formData.FirstName),
                ("Soyadı", formData.LastName),
                ("E-Posta", formData.Email),
                ("Telefon", formData.Phone),
                ("Yaşadığı Şehir", formData.LivingCity)
            }));

        sb.AppendLine("<div class='section'>");
        sb.AppendLine("<h3>Talep Bilgileri</h3>"); // PDF Eki yerine yeni başlık
        sb.AppendLine("<p style='text-align: left; white-space: pre; font-family: monospace;'>");
        sb.AppendLine(">> BU TALEP 7 GÜN İÇİN GEÇERLİDİR. EN MÜSAİT ANINIZDA MÜŞTERİYİ ARAYIP İŞİ KAPIN.\n" +
                        ">> NE KADAR HİZLİ DAVRANIRSANIZ SİZİN İÇİN O KADAR İYİ OLACAKTIR.\n" +
                        ">> BU HİZMET SİZ EMLAKÇILAR İÇİN TAMAMEN ÜCRETSİZDİR. VE ÜCRETSİZ KALACAKTIR.\n" +
                        ">> PÖRTFÖYÜNÜZÜ BİZİMLE DOLDURABİLİRSİNİZ.\n" +
                        ">> MÜŞTERİLERDEN İLAN ÜCRETİ VS TALEP ETMEYİNİZ.\n" +
                        ">> %2 KOMİSYON DIŞINDA FARKLI TALEPLERDE BULUNMAYINIZ.\n" +
                        ">> FORMU DOLDURAN MÜŞTERİMİZ ARANDIM, BAŞKA EMLAKÇI İLE ANLAŞTIM DERSE BİZE BU MAİL ÜZERİNDEN TALEP NUMARASINI YAZARAK BİLGİ GEÇEBİLİRSİNİZ.");
        sb.AppendLine("</p>");
        sb.AppendLine("</div>");


        sb.AppendLine("</div>");
        sb.AppendLine("<div class='footer'>Bu mesaj, bir sistem tarafından otomatik olarak gönderilmiştir.</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        // Oluşturulan HTML içeriğini Türkçe karakterleri değiştiren metot ile işliyoruz.
        return ReplaceTurkishCharacters(sb.ToString());
    }

    private byte[] GeneratePdf(CreateSalesRequestCommandRequest formData)
    {
        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4, 40, 40, 40, 40);
        var writer = PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        // Modern başlık ve logo için ortak bir arka plan alanı oluştur
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.WHITE);
        var sectionTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(0, 51, 102));

        // Başlık ve logo bir arada olacak şekilde ayarlanan table
        var headerTable = new PdfPTable(1)
        {
            WidthPercentage = 100,
            SpacingBefore = 10f,
            SpacingAfter = 20f
        };

        // 1. Hücre: Logo
        var logoCell = new PdfPCell()
        {
            Border = Rectangle.NO_BORDER,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            PaddingTop = 20f
        };
        try
        {
            // Logo dosyasının yolu
            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "favlogo.png");

            // Logo resmini al
            var logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.Alignment = Element.ALIGN_CENTER;
            logo.ScaleToFit(100f, 100f); // Boyutlandırma
            logoCell.AddElement(logo); // Logoyu hücreye ekle
        }
        catch (Exception)
        {
            // Logo yüklenemeyince devam et
            Console.WriteLine("Logo yüklenemedi.");
        }

        // 2. Hücre: Başlık
        var headerCell = new PdfPCell(new Phrase(ReplaceTurkishCharacters("Yeni Mülk Satış Talebi"), titleFont))
        {
            BackgroundColor = new BaseColor(0, 51, 102),
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 15,
            Border = iTextSharp.text.Rectangle.NO_BORDER
        };

        // Hücreleri tabloya ekle
        headerTable.AddCell(logoCell); // Logo hücresini ekle
        headerTable.AddCell(headerCell); // Başlık hücresini ekle

        // Tabloyu PDF'ye ekle
        document.Add(headerTable);

        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);

        // Kategori Bilgileri
        AddSectionToPdf(document, "Kategori Bilgileri", new List<(string, string)>
            {
                ("Kategori", formData.SelectCategory),
                ("Durum", formData.SelectStatus)
            }, sectionTitleFont, normalFont);

        // Mülk Bilgileri
        if (!string.IsNullOrEmpty(formData.Rooms) || !string.IsNullOrEmpty(formData.Area) ||
            !string.IsNullOrEmpty(formData.BuildingAge) || !string.IsNullOrEmpty(formData.Floor) ||
            !string.IsNullOrEmpty(formData.Bathrooms) || !string.IsNullOrEmpty(formData.Price) ||
            !string.IsNullOrEmpty(formData.ResidentialCity) || !string.IsNullOrEmpty(formData.ResidentialDistrict) ||
            !string.IsNullOrEmpty(formData.ResidentialVillage))
        {
            AddSectionToPdf(document, "Mülk Bilgileri", new List<(string, string)>
                {
                    ("Oda Sayısı", formData.Rooms),
                    ("Metrekare", formData.Area),
                    ("Bina Yaşı", formData.BuildingAge),
                    ("Kat", formData.Floor),
                    ("Banyo Sayısı", formData.Bathrooms),
                    ("Fiyat", formData.Price),
                    ("Konum", $"{formData.ResidentialCity}, {formData.ResidentialDistrict}, {formData.ResidentialVillage}")
                }, sectionTitleFont, normalFont);
        }

        // Arazi Bilgileri
        if (!string.IsNullOrEmpty(formData.LandAda) || !string.IsNullOrEmpty(formData.LandParsel) ||
            !string.IsNullOrEmpty(formData.LandArea) || !string.IsNullOrEmpty(formData.Slope) ||
            !string.IsNullOrEmpty(formData.RoadCondition) || !string.IsNullOrEmpty(formData.DistanceToSettlement) ||
            !string.IsNullOrEmpty(formData.ZoningStatus) || !string.IsNullOrEmpty(formData.LandCity) ||
            !string.IsNullOrEmpty(formData.LandDistrict) || !string.IsNullOrEmpty(formData.LandVillage))
        {
            AddSectionToPdf(document, "Arazi Bilgileri", new List<(string, string)>
                {
                    ("Ada", formData.LandAda),
                    ("Parsel", formData.LandParsel),
                    ("Metrekare", formData.LandArea),
                    ("Meyil Durumu", formData.Slope),
                    ("Yol Durumu", formData.RoadCondition),
                    ("Yerleşim Uzaklığı", formData.DistanceToSettlement),
                    ("İmar Durumu", formData.ZoningStatus),
                    ("Fiyat", formData.LandPrice),
                    ("Konum", $"{formData.LandCity}, {formData.LandDistrict}, {formData.LandVillage}")
                }, sectionTitleFont, normalFont);
        }

        // Mülk Sahibi Bilgileri
        AddSectionToPdf(document, "Mülk Sahibi Bilgileri", new List<(string, string)>
            {
                ("Adı", formData.FirstName),
                ("Soyadı", formData.LastName),
                ("E-Posta", formData.Email),
                ("Telefon", formData.Phone),
                ("Yaşadığı Şehir", formData.LivingCity)
            }, sectionTitleFont, normalFont);

        // Alt bilgi
        var footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
        string footerText = ReplaceTurkishCharacters(">> BU TALEP 7 GÜN İÇİN GEÇERLİDİR. EN MÜSAİT ANINIZDA MÜŞTERİYİ ARAYIP İŞİ KAPIN.\n" +
            ">> NE KADAR HİZLİ DAVRANIRSANIZ SİZİN İÇİN O KADAR İYİ OLACAKTIR.\n" +
            ">> BU HİZMET SİZ EMLAKÇILAR İÇİN TAMAMEN ÜCRETSİZDİR. VE ÜCRETSİZ KALACAKTIR.\n" +
            ">> PÖRTFÖYÜNÜZÜ BİZİMLE DOLDURABİLİRSİNİZ.\n" +
            ">> MÜŞTERİLERDEN İLAN ÜCRETİ VS TALEP ETMEYİNİZ.\n" +
            ">> %2 KOMİSYON DIŞINDA FARKLI TALEPLERDE BULUNMAYINIZ.\n" +
            ">> FORMU DOLDURAN MÜŞTERİMİZ ARANDIM, BAŞKA EMLAKÇI İLE ANLAŞTIM DERSE BİZE BU MAİL ÜZERİNDEN TALEP NUMARASINI YAZARAK BİLGİ GEÇEBİLİRSİNİZ.");

        Paragraph footer = new Paragraph(footerText, footerFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingBefore = 30f
        };
        document.Add(footer);

        document.Close();
        return memoryStream.ToArray();
    }
    private string BuildSectionHtml(string sectionTitle, List<(string label, string value)> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<div class='section'>");
        sb.AppendLine($"<h3>{sectionTitle}</h3>");
        sb.AppendLine("<table>");
        sb.AppendLine("<tr><th>Bilgi</th><th>Deger</th></tr>");
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.value))
            {
                sb.AppendLine($"<tr><td>{item.label}</td><td>{item.value}</td></tr>");
            }
        }
        sb.AppendLine("</table>");
        sb.AppendLine("</div>");
        return sb.ToString();
    }

    private void AddSectionToPdf(iTextSharp.text.Document document, string sectionTitle, List<(string label, string value)> items, Font sectionFont, Font textFont)
    {
        // Bölüm başlığını dönüştürerek ekleyin.
        Paragraph title = new Paragraph(ReplaceTurkishCharacters(sectionTitle), sectionFont)
        {
            SpacingBefore = 15f,
            SpacingAfter = 10f
        };
        document.Add(title);

        // İki sütunlu tablo oluştur
        PdfPTable table = new PdfPTable(2)
        {
            WidthPercentage = 100,
            SpacingAfter = 15f
        };

        // Tablo başlıkları
        var headerBackground = new BaseColor(0, 51, 102);
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);

        PdfPCell cellLabel = new PdfPCell(new Phrase("Bilgi", headerFont))
        {
            BackgroundColor = headerBackground,
            Padding = 8,
            HorizontalAlignment = Element.ALIGN_CENTER
        };
        PdfPCell cellValue = new PdfPCell(new Phrase("Deger", headerFont))
        {
            BackgroundColor = headerBackground,
            Padding = 8,
            HorizontalAlignment = Element.ALIGN_CENTER
        };

        table.AddCell(cellLabel);
        table.AddCell(cellValue);

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.value))
            {
                PdfPCell labelCell = new PdfPCell(new Phrase(ReplaceTurkishCharacters(item.label), textFont))
                {
                    Padding = 8,
                    BackgroundColor = new BaseColor(245, 245, 245)
                };
                PdfPCell valueCell = new PdfPCell(new Phrase(ReplaceTurkishCharacters(item.value), textFont))
                {
                    Padding = 8
                };

                table.AddCell(labelCell);
                table.AddCell(valueCell);
            }
        }

        document.Add(table);
    }



    private string BuildOwnerEmailBody()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset='utf-8'/>");
        sb.AppendLine("<style>");
        sb.AppendLine("body { font-family: 'Arial', sans-serif; background-color: #f4f8fb; margin: 0; padding: 0; }");
        sb.AppendLine(".container { width: 90%; max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 0 15px rgba(0,0,0,0.1); overflow: hidden; }");
        sb.AppendLine(".header { background: linear-gradient(to right, #003366, #004080); padding: 20px; text-align: center; }");
        sb.AppendLine(".header img { max-width: 80px; }");
        sb.AppendLine(".header h2 { color: #fff; margin: 10px 0 0 0; font-size: 24px; }");
        sb.AppendLine(".content { padding: 20px; color: #333; }");
        sb.AppendLine(".message { font-size: 16px; line-height: 1.6; margin-bottom: 20px; }");
        sb.AppendLine(".contact-info { background-color: #e9ecef; padding: 15px; border-radius: 8px; text-align: center; }");
        sb.AppendLine(".contact-info a { color: #003366; text-decoration: none; font-weight: bold; }");
        sb.AppendLine(".footer { text-align: center; padding: 15px; background-color: #f0f0f0; color: #003366; font-size: 12px; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class='container'>");
        sb.AppendLine("<div class='header'>");
        sb.AppendLine("<img src='https://i.hizliresim.com/sw39o6d.png' alt='Şirket Logosu' />");
        sb.AppendLine("<h2>Mülk Satış Talebiniz Alınmıştır</h2>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class='content'>");
        sb.AppendLine("<p class='message'>");
        sb.AppendLine("Talebiniz başarıyla alınmıştır. Talep öncelikle firmamızın paneline düşecek ve ardından onay için aranacaksınız. Gerekli onay gerçekleştikten sonra, mülkünüzün bulunduğu şehirdeki tüm emlakçılar iş emri olarak sunulacaktır.");
        sb.AppendLine("Emlakçılardan aldığınız geri dönüş doğrultusunda görüşleriniz için bizi arayınız:");
        sb.AppendLine("</p>");
        sb.AppendLine("<div class='contact-info'>");
        sb.AppendLine("<a href='tel:+902129555541'>+90 (212) 955 55 41</a>");
        sb.AppendLine("</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class='footer'>Bu mesaj, bir sistem tarafından otomatik olarak gönderilmiştir.</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return ReplaceTurkishCharacters(sb.ToString());
    }


    private string ReplaceTurkishCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input
            .Replace("Ç", "C")
            .Replace("ç", "c")
            .Replace("Ğ", "G")
            .Replace("ğ", "g")
            .Replace("İ", "I")
            .Replace("ı", "i")
            .Replace("Ö", "O")
            .Replace("ö", "o")
            .Replace("Ş", "S")
            .Replace("ş", "s")
            .Replace("Ü", "U")
            .Replace("ü", "u");
    }


}

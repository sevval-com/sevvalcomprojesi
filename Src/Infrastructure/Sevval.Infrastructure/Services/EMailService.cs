using Microsoft.Extensions.Configuration;
using Sevval.Application.Constants;
using Sevval.Application.Dtos.Email;
using Sevval.Application.Interfaces.IService;
using System.Net.Mail;

namespace Sevval.Infrastructure.Services
{
    public class EMailService : IEMailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public EMailService(IConfiguration configuration, SmtpClient smtpClient)
        {
            _configuration = configuration;
            _smtpClient = smtpClient;
        }



        public async Task<bool> SendPasswordResetEmailAsync(ForgotPasswordViewDto model, string code)
        {
            var message = new MailMessage(
                from: _configuration["Email:FromAddress"],
                to: model.Email,
                subject: "Şifre Sıfırlama Talebi",
                body: $@"
                    <div style='background-color: #f4f6f9; font-family: Arial, sans-serif; padding: 0; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); max-width: 600px; margin: auto; overflow: hidden;'>
                        <div style='background-color: #007bff; color: #fff; padding: 15px; text-align: center;'>
                            <img src='https://resmim.net/cdn/2024/11/25/Ddd4F7.png' alt='Şirket Logosu' style='width: 120px; display: block; margin: 0 auto;'>
                            <h1 style='font-size: 20px; margin: 10px 0 0;'>Şevval Emlak</h1>
                        </div>
                        <div style='background-color: #ffffff; padding: 20px;'>
                            <h2 style='color: #333; text-align: center; margin-top: 0;'>Şifre Sıfırlama Talebiniz</h2>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Merhaba,
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Hesabınız için bir şifre sıfırlama talebi aldık. Şifre sıfırlama kodunuz aşağıda yer almaktadır:
                            </p>
                            <div style='text-align: center; margin: 20px 0;'>
                                <span style='display: inline-block; background-color: #007bff; color: #fff; font-size: 16px; font-weight: bold; padding: 10px 20px; border-radius: 5px;'>{code}</span>
                            </div>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Eğer bu talebi siz yapmadıysanız, lütfen bu e-postayı dikkate almayınız. Daha fazla bilgi için destek ekibimize ulaşabilirsiniz.
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Saygılarımızla,<br>
                                <strong>Şevval Emlak Ekibi</strong>
                            </p>
                        </div>
                        <div style='background-color: #f4f6f9; text-align: center; font-size: 12px; color: #999; padding: 10px 20px;'>
                            Bu bir otomatik mesajdır, lütfen yanıtlamayın.<br>
                            &copy; 2024 Şevval Emlak. Tüm hakları saklıdır.
                        </div>
                    </div>
                    ")
            {
                IsBodyHtml = true
            };

            return await SendEmailAsync(model.Email, message.Subject, message.Body);

        }


        public async Task<bool> SendVerificationEmailAsync(SendVerifyEmailDto model)
        {
            var message = new MailMessage(
                from: _configuration["Email:FromAddress"],
                to: model.Email,
                subject: "E-posta Doğrulama Talebi",
                body: $@"
                    <div style='background-color: #f4f6f9; font-family: Arial, sans-serif; padding: 0; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); max-width: 600px; margin: auto; overflow: hidden;'>
                        <!-- Üst Kısım (Logo ve Başlık) -->
                        <div style='background-color: #007bff; color: #fff; padding: 15px; text-align: center;'>
                            <img src='https://resmim.net/cdn/2024/11/25/Ddd4F7.png' alt='Şirket Logosu' style='width: 120px; display: block; margin: 0 auto;'>
                            <h1 style='font-size: 22px; margin: 10px 0 0; font-weight: bold;'>Şevval Emlak</h1>
                        </div>
                        <!-- İçerik -->
                        <div style='background-color: #ffffff; padding: 20px;'>
                            <h2 style='color: #333; text-align: center; margin-top: 0;'>E-posta Doğrulama Talebiniz</h2>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Merhaba,
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Hesabınızı aktif hale getirmek için aşağıdaki butona tıklayarak e-postanızı doğrulayabilirsiniz:
                            </p>
                            <div style='text-align: center; margin: 20px 0;'>
                                <a href='{model.Link}' style='
                                    display: inline-block; 
                                    background-color: #007bff; 
                                    color: white; 
                                    font-size: 16px;
                                    font-weight: bold;
                                    padding: 12px 20px; 
                                    text-decoration: none; 
                                    border-radius: 5px;'>
                                    E-postamı Doğrula
                                </a>
                            </div>
        
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Daha fazla bilgiye ihtiyaç duyarsanız, lütfen bizimle iletişime geçin.
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Saygılarımızla,<br>
                                <strong>Şevval Emlak Ekibi</strong>
                            </p>
                        </div>
                        <!-- Alt Bilgi -->
                        <div style='background-color: #f4f6f9; text-align: center; font-size: 12px; color: #999; padding: 15px;'>
                            Bu bir otomatik mesajdır, lütfen yanıtlamayın.<br>
                            &copy; 2024 Şevval Emlak. Tüm hakları saklıdır.
                        </div>
                    </div>
                    ")
            {
                IsBodyHtml = true
            };

            return await SendEmailAsync(model.Email, message.Subject, message.Body);

        }



        public async Task<bool> SendEstateConfirmationEmailAsync(SendEstateConfirmationDto model)
        {
            var message = new MailMessage(
                from: _configuration["Email:FromAddress"],
                to: GeneralConstants.SendEstateConfirmationEmail,
                subject: "Yeni Kurumsal Üyelik Başvurusu",
                body: $@"
        <div style='background-color: #f5f8fa; padding: 20px; font-family: Arial, sans-serif;'>
            <div style='max-width: 800px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                <!-- Header -->
                <div style='text-align: center; border-bottom: 2px solid #0066cc; padding-bottom: 20px; margin-bottom: 30px;'>
                    <img src='https://resmim.net/cdn/2024/11/25/Ddd4F7.png' alt='Şevval Emlak' style='width: 150px; margin-bottom: 20px;' />
                    <h1 style='color: #0066cc; margin: 0;'>Yeni Kurumsal Üyelik Başvurusu</h1>
                    <p style='color: #666; margin-top: 10px;'>Başvuru Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}</p>
                </div>

                <!-- Kişisel Bilgiler -->
                <div style='margin-bottom: 30px; background-color: #f8f9fa; padding: 20px; border-radius: 8px;'>
                    <h2 style='color: #0066cc; font-size: 20px; margin-bottom: 15px;'>Kişisel Bilgiler</h2>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <td style='padding: 8px; color: #666; width: 200px;'>Ad Soyad:</td>
                            <td style='padding: 8px; color: #333;'>{model.FirstName} {model.LastName}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; color: #666;'>E-posta:</td>
                            <td style='padding: 8px; color: #333;'>{model.Email}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; color: #666;'>Şifre:</td>
                            <td style='padding: 8px; color: #333;'>{model.Password}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; color: #666;'>Telefon:</td>
                            <td style='padding: 8px; color: #333;'>{model.PhoneNumber}</td>
                        </tr>
                    </table>
                </div>

                <!-- Şirket Bilgileri -->
                <div style='margin-bottom: 30px; background-color: #f8f9fa; padding: 20px; border-radius: 8px;'>
                    <h2 style='color: #0066cc; font-size: 20px; margin-bottom: 15px;'>Şirket Bilgileri</h2>
                    <table style='width: 100%; border-collapse: collapse;'>
                        <tr>
                            <td style='padding: 8px; color: #666; width: 200px;'>Şirket Adı:</td>
                            <td style='padding: 8px; color: #333;'>{model.CompanyName}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; color: #666;'>İl/İlçe:</td>
                            <td style='padding: 8px; color: #333;'>{model.City} / {model.District}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; color: #666;'>Adres:</td>
                            <td style='padding: 8px; color: #333;'>{model.Address}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px; color: #666;'>Referans:</td>
                            <td style='padding: 8px; color: #333;'>{model.Reference ?? "Belirtilmedi"}</td>
                        </tr>
                    </table>
                </div>

                <!-- Belgeler -->
                <div style='margin-bottom: 30px; background-color: #f8f9fa; padding: 20px; border-radius: 8px;'>
                    <h2 style='color: #0066cc; font-size: 20px; margin-bottom: 15px;'>Yüklenen Belgeler</h2>
                    <div style='margin-bottom: 15px;'>
                        <p style='margin: 5px 0; color: #666;'>Seviye 5 Belgesi:</p>
                        <a href='{model.Level5CertificatePath}' style='color: #0066cc; text-decoration: underline;' target='_blank'>
                            Belgeyi Görüntüle
                        </a>
                    </div>
                    <div style='margin-bottom: 15px;'>
                        <p style='margin: 5px 0; color: #666;'>Vergi Levhası:</p>
                        <a href='{model.TaxPlatePath}' style='color: #0066cc; text-decoration: underline;' target='_blank'>
                            Belgeyi Görüntüle
                        </a>
                    </div>
                    <div>
                        <p style='margin: 5px 0; color: #666;'>Şirket Logosu:</p>
                        <a href='{model.ProfilePicturePath}' style='color: #0066cc; text-decoration: underline;' target='_blank'>
                            Belgeyi Görüntüle
                        </a>
                    </div>
                </div>

                <!-- Onay ve Ret Butonları -->
                <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 2px solid #eee;'>
                    <p style='color: #666; margin-bottom: 20px;'>Başvuru bilgilerini inceledikten sonra onaylamak veya reddetmek için aşağıdaki butonları kullanınız:</p>
                    <div style='display: flex; justify-content: center; gap: 20px;'>
                        <a href='{model.ConfirmUrl}' style='
                            display: inline-block;
                            background-color: #28a745;
                            color: white;
                            padding: 15px 40px;
                            text-decoration: none;
                            border-radius: 5px;
                            font-weight: bold;
                            font-size: 16px;
                            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                        '>
                            Başvuruyu Onayla
                        </a>
                        <a href='{model.RejectUrl}' style='
                            display: inline-block;
                            background-color: #dc3545;
                            color: white;
                            padding: 15px 40px;
                            text-decoration: none;
                            border-radius: 5px;
                            font-weight: bold;
                            font-size: 16px;
                            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                        '>
                            Başvuruyu Reddet
                        </a>
                    </div>
                </div>

                <!-- Footer -->
                <div style='margin-top: 40px; text-align: center; color: #666; font-size: 14px; border-top: 2px solid #eee; padding-top: 20px;'>
                    <p>Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.</p>
                </div>
            </div>
        </div>
    ")
            {
                IsBodyHtml = true
            };

            return await SendEmailAsync(GeneralConstants.SendEstateConfirmationEmail, message.Subject, message.Body);

        }

        public async Task<bool> SendConfirmeInformationMailToEstate(string email)
        {
            var message = new MailMessage(
                from: _configuration["Email:FromAddress"],
                to: email,
                subject: "Girişiniz Başarıyla Onaylandı!",
                body: $@"
                    <div style='background-color: #f4f6f9; font-family: Arial, sans-serif; padding: 0; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); max-width: 600px; margin: auto; overflow: hidden;'>
                        <!-- Header -->
                        <div style='background-color: #28a745; color: #fff; padding: 15px; text-align: center;'>
                            <img src='https://resmim.net/cdn/2024/11/25/Ddd4F7.png' alt='Şirket Logosu' style='width: 120px; display: block; margin: 0 auto;'>
                            <h1 style='font-size: 20px; margin: 10px 0 0;'>Şevval Emlak</h1>
                            <p style='font-size: 14px; margin-top: 5px;'></p>
                        </div>
                        <!-- Body -->
                        <div style='background-color: #ffffff; padding: 20px;'>
                            <h2 style='color: #333; text-align: center; margin-top: 0;'>Tebrikler!</h2>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Sayın Kullanıcı,
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Hesabınız başarıyla onaylanmıştır. Artık Şevval Emlak platformunun tüm avantajlarından yararlanabilirsiniz.
                            </p>
                            <div style='text-align: center; margin: 20px 0;'>
                                <a href='{GeneralConstants.BaseUrl}/Account/Login?Type=KURUMSAL' style='
                                    display: inline-block;
                                    background-color: #28a745;
                                    color: white;
                                    padding: 15px 40px;
                                    text-decoration: none;
                                    border-radius: 5px;
                                    font-weight: bold;
                                    font-size: 16px;
                                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                                '>
                                    Giriş Yap
                                </a>
                            </div>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Sorularınız veya yardıma ihtiyacınız olursa, lütfen bizimle iletişime geçmekten çekinmeyin.
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Saygılarımızla,<br>
                                <strong>Şevval Emlak Ekibi</strong>
                            </p>
                        </div>
                        <div style='background-color: #f4f6f9; text-align: center; font-size: 12px; color: #999; padding: 10px 20px;'>
                            Bu bir otomatik mesajdır, lütfen yanıtlamayın.<br>
                            &copy; 2024 Şevval Emlak. Tüm hakları saklıdır.
                        </div>
                    </div>
                    ")
            {
                IsBodyHtml = true
            };

            return await SendEmailAsync(email, message.Subject, message.Body);

        }



        public async Task<bool> SendAwaitingApprovalMailToEstate(string email)
        {
            var message = new MailMessage(
                from: _configuration["Email:FromAddress"],
                to: email,
                subject: "Girişiniz Onay Bekliyor!",
                body: $@"
                    <div style='background-color: #f4f6f9; font-family: Arial, sans-serif; padding: 0; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); max-width: 600px; margin: auto; overflow: hidden;'>
                        <!-- Header -->
                        <div style='background-color: #004aad; color: #fff; padding: 15px; text-align: center;'>
                            <img src='https://resmim.net/cdn/2024/11/25/Ddd4F7.png' alt='Şirket Logosu' style='width: 120px; display: block; margin: 0 auto;'>
                            <h1 style='font-size: 20px; margin: 10px 0 0;'>Şevval Emlak</h1>
                            <p style='font-size: 14px; margin-top: 5px;'>Hesabınız onay bekliyor!</p>
                        </div>
                        <!-- Body -->
                        <div style='background-color: #ffffff; padding: 20px;'>
                            <h2 style='color: #333; text-align: center; margin-top: 0;'>Üyelik Formunuz Başarıyla İletildi</h2>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Sayın Kullanıcı,
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Hesabınızın onaylanma süreci başarıyla başlatılmıştır. 
                                <strong style='color: #004aad;'>48 saat</strong> içerisinde üyeliğiniz kontrol edilecektir. Herhangi bir sorun olmaması durumunda hesabınız onaylanarak tarafınıza bilgilendirme yapılacaktır.
                            </p>
                            <div style='text-align: center; margin: 20px 0;'>
                        <a href='mailto:sevvaldestek@gmail.com?subject=Destek Talebi&body=Merhaba, destek talebim ile ilgili olarak iletişime geçiyorum.' style='
                            display: inline-block; 
                            background-color: #004aad; 
                            color: white; 
                            font-size: 14px;
                            font-weight: bold;
                            padding: 12px 20px; 
                            text-decoration: none; 
                            border-radius: 5px;'>
                            Destek Talebi Oluştur
                        </a>
                    </div>

                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Her türlü soru ve görüşleriniz için destek ekibimizle iletişime geçebilirsiniz.
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Saygılarımızla,<br>
                                <strong>Şevval Emlak</strong>
                            </p>
                        </div>
                        <!-- Footer -->
                        <div style='background-color: #f4f6f9; text-align: center; font-size: 12px; color: #999; padding: 10px 20px;'>
                            Bu bir otomatik mesajdır, lütfen yanıtlamayın.<br>
                            &copy; 2024 Şevval Emlak. Tüm hakları saklıdır.
                        </div>
                    </div>
                    ")
            {
                IsBodyHtml = true
            };

            return await SendEmailAsync(email, message.Subject, message.Body);

        }

        public async Task<bool> SendRejectedMailToEstate(string email)
        {
            var message = new MailMessage(
                from: _configuration["Email:FromAddress"],
                to: email,
                subject: "Üyelik Başvurunuz Değerlendirildi",
                body: $@"
                    <div style='background-color: #f4f6f9; font-family: Arial, sans-serif; padding: 0; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); max-width: 600px; margin: auto; overflow: hidden;'>
                        <!-- Header -->
                        <div style='background-color: #dc3545; color: #fff; padding: 15px; text-align: center;'>
                            <img src='https://resmim.net/cdn/2024/11/25/Ddd4F7.png' alt='Şirket Logosu' style='width: 120px; display: block; margin: 0 auto;'>
                            <h1 style='font-size: 20px; margin: 10px 0 0;'>Şevval Emlak</h1>
                            <p style='font-size: 14px; margin-top: 5px;'>Üyelik Başvurusu Değerlendirilmesi</p>
                        </div>
                        <!-- Body -->
                        <div style='background-color: #ffffff; padding: 20px;'>
                            <h2 style='color: #333; text-align: center; margin-top: 0; color: #dc3545;'>Üyelik Başvurunuz Reddedildi</h2>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Sayın Kullanıcı,
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Şirketimize yapmış olduğunuz kurumsal üyelik başvurusu, değerlendirme sonucunda 
                                <strong style='color: #dc3545;'>reddedilmiştir</strong>. 
                            </p>
                            <div style='background-color: #f8f9fa; border-left: 4px solid #dc3545; padding: 15px; margin: 20px 0;'>
                                <p style='color: #555; font-size: 14px; line-height: 1.6; margin: 0;'>
                                    <strong>Olası Ret Sebepleri:</strong>
                                    <ul style='color: #555; font-size: 14px; line-height: 1.6; padding-left: 20px;'>
                                        <li>Eksik veya hatalı belge yüklenmesi</li>
                                        <li>Şirket bilgilerinin doğrulanamaması</li>
                                        <li>Başvuru kriterlerimizi karşılamaması</li>
                                    </ul>
                                </p>
                            </div>
                            <div style='text-align: center; margin: 20px 0;'>
                                <a href='mailto:sevvalemlakiletisim@gmail.com?subject=Üyelik Başvurusu Red Sebebi Hakkında Bilgi&body=Merhaba, üyelik başvurumun reddedilme sebebi hakkında detaylı bilgi almak istiyorum.' style='
                                    display: inline-block; 
                                    background-color: #dc3545; 
                                    color: white; 
                                    font-size: 14px;
                                    font-weight: bold;
                                    padding: 12px 20px; 
                                    text-decoration: none; 
                                    border-radius: 5px;'>
                                    Ret Sebebi Hakkında Bilgi Al
                                </a>
                            </div>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Başvurunuzun reddedilmesi durumunda, eksiklikleri tamamlayarak tekrar başvuruda bulunabilirsiniz. 
                                Detaylı bilgi almak için destek ekibimizle iletişime geçebilirsiniz.
                            </p>
                            <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                                Saygılarımızla,<br>
                                <strong>Şevval Emlak</strong>
                            </p>
                        </div>
                        <div style='background-color: #f4f6f9; text-align: center; font-size: 12px; color: #999; padding: 10px 20px;'>
                            Bu bir otomatik mesajdır, lütfen yanıtlamayın.<br>
                            &copy; 2024 Şevval Emlak. Tüm hakları saklıdır.
                        </div>
                    </div>
                    ")
            {
                IsBodyHtml = true
            };

            return await SendEmailAsync(email, message.Subject, message.Body);

        }

        public async Task<bool> SendConsultantInvitationEmailAsync(ConsultantInvitationEmailDto model)
        {
            var subject = "Danışman Daveti - Şevval Emlak";
            var body = $@"
        <div style='background-color: #f4f6f9; font-family: Arial, sans-serif; padding: 0; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.1); max-width: 600px; margin: auto; overflow: hidden;'>
            <!-- Üst Kısım (Logo ve Başlık) -->
            <div style='background-color: #007bff; color: #fff; padding: 15px; text-align: center;'>
                <img src='https://resmim.net/cdn/2024/11/25/Ddd4F7.png' alt='Şirket Logosu' style='width: 120px; display: block; margin: 0 auto;'>
                <h1 style='font-size: 22px; margin: 10px 0 0; font-weight: bold;'>Şevval Emlak</h1>
            </div>
            <!-- İçerik -->
            <div style='background-color: #ffffff; padding: 20px;'>
                <h2 style='color: #333; text-align: center; margin-top: 0;'>Danışmanlık Davetiniz</h2>
                <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                    Merhaba {model.FirstName} {model.LastName},
                </p>
                <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                    <span style='color: #007bff; font-size: 16px; font-weight: bold;'>{model.CompanyName}</span> firması sizi danışman olarak ekibine katılmaya davet ediyor. 
                </p>
                <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                    Hesabınızı aktifleştirmek ve daveti kabul etmek için aşağıdaki butona tıklayabilirsiniz:
                </p>
                <div style='text-align: center; margin: 20px 0;'>
                    <a href='{model.SetPasswordUrl}' style='
                        display: inline-block; 
                        background-color: #007bff; 
                        color: white; 
                        font-size: 16px;
                        font-weight: bold;
                        padding: 12px 20px; 
                        text-decoration: none; 
                        border-radius: 5px;'>
                        Hesabı Aktifleştir
                    </a>
                </div>
                <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                    Bu bağlantı 7 gün boyunca geçerli olacaktır. Geçerlilik süresi sonunda yeniden bir davet almanız gerekebilir.
                </p>
                <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                    Daha fazla bilgi için bizimle iletişime geçebilirsiniz.
                </p>
                <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                    Saygılarımızla,<br>
                    <strong>Şevval Emlak Ekibi</strong>
                </p>
            </div>
            <!-- Alt Bilgi -->
            <div style='background-color: #f4f6f9; text-align: center; font-size: 12px; color: #999; padding: 15px;'>
                Bu bir otomatik mesajdır, lütfen yanıtlamayın.<br>
                &copy; 2024 Şevval Emlak. Tüm hakları saklıdır.
            </div>
        </div>";

            return await SendEmailAsync(model.Email, subject, body);
        }


        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage(
                from: _configuration["Email:FromAddress"],
                to: to,
                subject: subject,
                body: body
            )
            {
                IsBodyHtml = true
            };

            try
            {
                await _smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}

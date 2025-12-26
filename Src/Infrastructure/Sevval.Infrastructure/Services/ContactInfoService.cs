using GridBox.Solar.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.ContactInfo.Queries.GetContactInfo;
using Sevval.Application.Interfaces.Services;
using Sevval.Persistence.Context.sevvalemlak.Models;

namespace Sevval.Infrastructure.Services
{
    public class ContactInfoService :  IContactInfoService
    {
        private readonly IReadRepository<AboutUsContent> _readRepository;

        public ContactInfoService(IReadRepository<AboutUsContent> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<ApiResponse<GetContactInfoQueryResponse>> GetContactInfoAsync(CancellationToken cancellationToken)
        {
                // Defaults
                var defaultAddress = "Güney Mh, Saatçi Sk, No:9/A Kocaeli/Körfez";
                var defaultPhone = "0212 955 55 41";
                var defaultEmail = "sevvaltalep@gmail.com";
                var defaultWhatsApp = "905070775666";
                var defaultEtbisImage = "data:image/jpeg;base64, iVBORw0KGgoAAAANSUhEUgAAAIIAAACWCAYAAAASRFBwAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAEWRSURBVHhe7Z0HuF1F9fYhICEQEoqAdBCkCSgkEDAUEQREEKSI9K4oiJRI7yBFUVA6CAihKb1jRCBA2s2tyU0hCem99w7zrd8+5913nX3nlJsCfz59n+cl3Gl7n73XzKy1Zs3sVQxhRfBvf/tbADNnzgwbbLBBs/xu3bol+R433XRTs3L7779/PjeEc889t1m+5zvvvJOU+/TTT8Nqq62WpF155ZVJGth1112b1RHXWWedMGnSpKTcc889l6a/+eabSZrH7bffXlAXfu9738vnNmHu3LnhG9/4RpJ//PHH51ObMHLkyPC1r30tyf/d736XTy2NefPmhU033TSpc9xxx+VTQ/jJT36S3sty8kVjglhmi1lOEHr37p3ke8QEoWPHjvncEH7zm980y/fs3r17Um7KlClp2hVXXJGkgW222aagfJaLFi1Kyr300ktp2n/+858kzeOOO+4oqAe//e1v53MLoZd28skn51OaMGvWrFQQrrvuunxqeWy++eZJnS9EELbddttw6qmntog//OEP0wbLCcJll10WnnzyyQLecMMNzdq88MIL0/zvf//7zdrh5arsXXfdlZR76KGHwmmnnZak8YBV/1e/+lWSduihh6b1O3XqlKSdeeaZyT1T7g9/+EPa5p///Oe0vojAKl8kTejTp09S7pFHHgnt27dPrrPXXns1a4e2NXIdfvjhzfJj5B7XW2+9pE5MENZee+1E6LL3V4onnXRSWGuttZL6xkJB+MUvfpG/ROWorq5WY2UFIcZrr722qePRt2/faFmRFygccMABSdrXv/71fEoIt912W1p2+PDhSVpDQ0Oa9tRTTyVpoHXr1knaT3/603xKCD/4wQ/SsmK5YfyMM85oVmdlMCYIW2+9dT6lZdhiiy3UbqEgnH766fkileO9995TY8skCLfccktSx4PhPlZWPOaYY/IlQ9rTv/nNb4alS5cmabfeemtatl+/fknaRx99lKb9/e9/T9LAxhtvnKSdcsop+ZT4kHvRRRflc+Ng5MnWWRmMCcKWW26ZTnGVYsGCBWGzzTZTu8UF4fLLL08ecjGOHj06KVdOEA466KBE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";
                var defaultEtbisUrl = "https://etbis.eticaret.gov.tr/sitedogrulama/7788668188421678";

                // Load overrides from AboutUsContents where key starts with "contact:"
                var overrideList = await _readRepository
                    .Queryable()
                    .AsNoTracking()
                    .Where(c => c.Key.StartsWith("contact:"))
                    .ToListAsync(cancellationToken);

                var overrides = overrideList.ToDictionary(
                    k => k.Key.Replace("contact:", string.Empty),
                    v => v.Content);

                var contactInfo = new GetContactInfoQueryResponse
                {
                    Address = overrides.TryGetValue("address", out var address) ? address : defaultAddress,
                    Phone = overrides.TryGetValue("phone", out var phone) ? phone : defaultPhone,
                    Email = overrides.TryGetValue("email", out var email) ? email : defaultEmail,
                    WhatsAppNumber = overrides.TryGetValue("whatsAppNumber", out var wa) ? wa : defaultWhatsApp,
                    EtbisImageData = overrides.TryGetValue("etbisImageData", out var etbisImg) ? etbisImg : defaultEtbisImage,
                    EtbisUrl = overrides.TryGetValue("etbisUrl", out var etbisUrl) ? etbisUrl : defaultEtbisUrl
                };

            return new ApiResponse<GetContactInfoQueryResponse>
            {
                 Code = 200,
                 Data = contactInfo,
                 IsSuccessfull = true,
                 Message = "İletişim bilgileri başarıyla getirildi."

            };
        }
    }
}

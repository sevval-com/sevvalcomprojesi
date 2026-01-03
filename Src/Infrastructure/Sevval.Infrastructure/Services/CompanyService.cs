using AutoMapper;
using GridBox.Solar.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Company.Queries.GetCompanyByName;
using Sevval.Application.Features.Company.Queries.GetTotalCompanyCount;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;
using Sevval.Application.Interfaces.Services;
using Sevval.Application.Utilities;
using Sevval.Domain.Entities;
using System.Globalization;

namespace Sevval.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IReadRepository<ApplicationUser> _readRepository;
        private readonly IMapper _mapper;
        private readonly IReadRepository<IlanModel> _readAnnouncementRepository;
        private readonly IReadRepository<ConsultantInvitation> _consultantInvitationRepository;

        private static string NormalizeForSearch(string? input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
        
            var normalized = input
                .ToLower(new CultureInfo("tr-TR")) // önce Türkçe küçük harfe çevir
                .Replace("ı", "i")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ç", "c");
        
            normalized = new string(normalized.Where(char.IsLetterOrDigit).ToArray()); // harf ve rakam dışını temizle
        
            return normalized.ToUpperInvariant(); // hepsini invariant uppercase yap
        }

        public CompanyService(
            IReadRepository<ApplicationUser> readRepository,
            IMapper mapper,
            IReadRepository<IlanModel> readAnnouncementRepository,
            IReadRepository<ConsultantInvitation> consultantInvitationRepository)
        {
            _readRepository = readRepository;
            _mapper = mapper;
            _readAnnouncementRepository = readAnnouncementRepository;
            _consultantInvitationRepository = consultantInvitationRepository;
        }

        public async Task<ApiResponse<List<GetCompaniesQueryResponse>>> GetCompanies(GetCompaniesQueryRequest request, CancellationToken cancellationToken)
        {
            // 1️⃣ Firmaları filtrele
            var companiesQuery = _readRepository.Queryable().AsNoTracking()
                .Where(u => !u.IsConsultant &&
                           (u.UserTypes == "Emlakçı" || u.UserTypes == "Kurumsal" || u.UserTypes == "Vakıf"
                           || u.UserTypes == "İnşaat" || u.UserTypes == "Banka"));

            if (!string.IsNullOrEmpty(request.Search))
            {
                var normalizedTerm = NormalizeForSearch(request.Search);
                var likeTerm = $"%{request.Search}%";
                companiesQuery = companiesQuery.Where(c =>
                    ((c.CompanyName ?? string.Empty)
                        .Replace("İ", "I").Replace("I", "I").Replace("ı", "i")
                        .Replace("Ğ", "G").Replace("ğ", "g")
                        .Replace("Ü", "U").Replace("ü", "u")
                        .Replace("Ş", "S").Replace("ş", "s")
                        .Replace("Ö", "O").Replace("ö", "o")
                        .Replace("Ç", "C").Replace("ç", "c")
                        .ToUpper()
                        .Replace(" ", "")
                        .Replace("'", "").Replace("’", "").Replace("`", "")
                        .Replace("-", "").Replace("_", "")
                        .Replace(".", "").Replace(",", "").Replace("/", "")
                        .Replace("(", "").Replace(")", ""))
                        .Contains(normalizedTerm)
                    || EF.Functions.Like(EF.Functions.Collate((c.CompanyName ?? string.Empty), "Turkish_CI_AI"), likeTerm)
                    || ((c.City ?? string.Empty)
                        .Replace("İ", "I").Replace("I", "I").Replace("ı", "i")
                        .Replace("Ğ", "G").Replace("ğ", "g")
                        .Replace("Ü", "U").Replace("ü", "u")
                        .Replace("Ş", "S").Replace("ş", "s")
                        .Replace("Ö", "O").Replace("ö", "o")
                        .Replace("Ç", "C").Replace("ç", "c")
                        .ToUpper()
                        .Replace(" ", "")
                        .Replace("'", "").Replace("’", "").Replace("`", "")
                        .Replace("-", "").Replace("_", "")
                        .Replace(".", "").Replace(",", "").Replace("/", "")
                        .Replace("(", "").Replace(")", ""))
                        .Contains(normalizedTerm)
                    || EF.Functions.Like(EF.Functions.Collate((c.City ?? string.Empty), "Turkish_CI_AI"), likeTerm)
                    || ((c.District ?? string.Empty)
                        .Replace("İ", "I").Replace("I", "I").Replace("ı", "i")
                        .Replace("Ğ", "G").Replace("ğ", "g")
                        .Replace("Ü", "U").Replace("ü", "u")
                        .Replace("Ş", "S").Replace("ş", "s")
                        .Replace("Ö", "O").Replace("ö", "o")
                        .Replace("Ç", "C").Replace("ç", "c")
                        .ToUpper()
                        .Replace(" ", "")
                        .Replace("'", "").Replace("’", "").Replace("`", "")
                        .Replace("-", "").Replace("_", "")
                        .Replace(".", "").Replace(",", "").Replace("/", "")
                        .Replace("(", "").Replace(")", ""))
                        .Contains(normalizedTerm)
                    || EF.Functions.Like(EF.Functions.Collate((c.District ?? string.Empty), "Turkish_CI_AI"), likeTerm));
            }
            else if (!string.IsNullOrEmpty(request.CompanyName))
            {
                var normalizedTerm = NormalizeForSearch(request.CompanyName);
                var likeTerm = $"%{request.CompanyName}%";
                companiesQuery = companiesQuery.Where(c =>
                    ((c.CompanyName ?? string.Empty)
                        .Replace("İ", "I").Replace("I", "I").Replace("ı", "i")
                        .Replace("Ğ", "G").Replace("ğ", "g")
                        .Replace("Ü", "U").Replace("ü", "u")
                        .Replace("Ş", "S").Replace("ş", "s")
                        .Replace("Ö", "O").Replace("ö", "o")
                        .Replace("Ç", "C").Replace("ç", "c")
                        .ToUpper()
                        .Replace(" ", "")
                        .Replace("'", "").Replace("’", "").Replace("`", "")
                        .Replace("-", "").Replace("_", "")
                        .Replace(".", "").Replace(",", "").Replace("/", "")
                        .Replace("(", "").Replace(")", ""))
                        .Contains(normalizedTerm)
                    || EF.Functions.Like(EF.Functions.Collate((c.CompanyName ?? string.Empty), "Turkish_CI_AI"), likeTerm)
                    || ((c.City ?? string.Empty)
                        .Replace("İ", "I").Replace("I", "I").Replace("ı", "i")
                        .Replace("Ğ", "G").Replace("ğ", "g")
                        .Replace("Ü", "U").Replace("ü", "u")
                        .Replace("Ş", "S").Replace("ş", "s")
                        .Replace("Ö", "O").Replace("ö", "o")
                        .Replace("Ç", "C").Replace("ç", "c")
                        .ToUpper()
                        .Replace(" ", "")
                        .Replace("'", "").Replace("’", "").Replace("`", "")
                        .Replace("-", "").Replace("_", "")
                        .Replace(".", "").Replace(",", "").Replace("/", "")
                        .Replace("(", "").Replace(")", ""))
                        .Contains(normalizedTerm)
                    || EF.Functions.Like(EF.Functions.Collate((c.City ?? string.Empty), "Turkish_CI_AI"), likeTerm)
                    || ((c.District ?? string.Empty)
                        .Replace("İ", "I").Replace("I", "I").Replace("ı", "i")
                        .Replace("Ğ", "G").Replace("ğ", "g")
                        .Replace("Ü", "U").Replace("ü", "u")
                        .Replace("Ş", "S").Replace("ş", "s")
                        .Replace("Ö", "O").Replace("ö", "o")
                        .Replace("Ç", "C").Replace("ç", "c")
                        .ToUpper()
                        .Replace(" ", "")
                        .Replace("'", "").Replace("’", "").Replace("`", "")
                        .Replace("-", "").Replace("_", "")
                        .Replace(".", "").Replace(",", "").Replace("/", "")
                        .Replace("(", "").Replace(")", ""))
                        .Contains(normalizedTerm)
                    || EF.Functions.Like(EF.Functions.Collate((c.District ?? string.Empty), "Turkish_CI_AI"), likeTerm));
            }

            if (!string.IsNullOrEmpty(request.Province))
                companiesQuery = companiesQuery.Where(c => c.City == request.Province);

            if (!string.IsNullOrEmpty(request.District))
                companiesQuery = companiesQuery.Where(c => c.District == request.District);

            // 2️⃣ Firmaları çek
            var companyList = await companiesQuery.ToListAsync(cancellationToken);
            var companyIds = companyList.Select(c => c.Id.ToString()).ToList();
            var companyEmails = companyList.Select(c => c.Email).Where(e => !string.IsNullOrEmpty(e)).ToList();

            // 3️⃣ Davetlileri çek
            var allInvitations = await _consultantInvitationRepository.Queryable()
                .Where(ci => companyIds.Contains(ci.InvitedBy))
                .ToListAsync(cancellationToken);

            var invitationsLookup = allInvitations
                .GroupBy(i => i.InvitedBy)
                .ToDictionary(g => g.Key, g => g.Select(i => i.Email).Where(e => !string.IsNullOrEmpty(e)).ToList());

            // 4️⃣ Tüm ilgili email'leri topla
            var allRelevantEmails = companyEmails.ToList();
            foreach (var inv in invitationsLookup.Values)
                allRelevantEmails.AddRange(inv);
            allRelevantEmails = allRelevantEmails.Distinct().ToList();

            // 5️⃣ İlan sayılarını email bazında çek
            var announcementCountsByEmail = await _readAnnouncementRepository.Queryable()
                .Where(a => a.Status != null && a.Status.ToLower() == "active" && a.Email != null && allRelevantEmails.Contains(a.Email))
                .GroupBy(a => a.Email)
                .Select(g => new { Email = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Email!, x => x.Count, cancellationToken);

            // 6️⃣ Her firma için toplam ilan sayısını hesapla
            var companyAnnouncementCounts = new Dictionary<string, int>();
            foreach (var company in companyList)
            {
                int count = 0;
                
                // Firma kendi ilanları
                if (!string.IsNullOrEmpty(company.Email) && announcementCountsByEmail.TryGetValue(company.Email, out var ownCount))
                    count += ownCount;

                // Davetli ilanları
                if (invitationsLookup.TryGetValue(company.Id.ToString(), out var invitedEmails))
                {
                    foreach (var email in invitedEmails)
                    {
                        if (announcementCountsByEmail.TryGetValue(email, out var invitedCount))
                            count += invitedCount;
                    }
                }

                companyAnnouncementCounts[company.Id.ToString()] = count;
            }

            // DEBUG LOG
            Console.WriteLine($"[CompanyService] Toplam firma: {companyList.Count}");
            Console.WriteLine($"[CompanyService] İlanı olan firma: {companyAnnouncementCounts.Count(x => x.Value > 0)}");
            Console.WriteLine($"[CompanyService] announcementCountsByEmail count: {announcementCountsByEmail.Count}");
            Console.WriteLine($"[CompanyService] allRelevantEmails count: {allRelevantEmails.Count}");

            // 7️⃣ Sıralama
            companyList = request.SortBy switch
            {
                "company_asc" => companyList.OrderBy(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "company_desc" => companyList.OrderByDescending(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "announcement_asc" => companyList.OrderBy(c => companyAnnouncementCounts.GetValueOrDefault(c.Id.ToString(), 0))
                                                 .ThenBy(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "announcement_desc" => companyList.OrderByDescending(c => companyAnnouncementCounts.GetValueOrDefault(c.Id.ToString(), 0))
                                                  .ThenBy(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "date_asc" => companyList.OrderBy(c => c.RegistrationDate).ToList(),
                "date_desc" => companyList.OrderByDescending(c => c.RegistrationDate).ToList(),
                // Varsayılan: Önce ilan sayısına göre (çoktan aza), sonra tarihe göre (yeniden eskiye)
                _ => companyList.OrderByDescending(c => companyAnnouncementCounts.GetValueOrDefault(c.Id.ToString(), 0))
                                .ThenByDescending(c => c.RegistrationDate).ToList()
            };
            
            // DEBUG: Sıralama sonrası ilk 10 firmayı logla
            Console.WriteLine($"[CompanyService] Sıralama sonrası ilk 10:");
            foreach (var company in companyList.Take(10))
            {
                var count = companyAnnouncementCounts.GetValueOrDefault(company.Id.ToString(), 0);
                Console.WriteLine($"  {company.CompanyName}: {count} ilan, Tarih: {company.RegistrationDate.ToString("dd.MM.yyyy")}");
            }

            // 8️⃣ Pagination
            var pagedList = companyList
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .ToList();

            var mapped = _mapper.Map<List<GetCompaniesQueryResponse>>(pagedList);

            // Batch olarak tüm firmaların ilan sayılarını hesapla (N+1 problemi çözümü)
            var pagedCompanyIds = pagedList.Select(c => c.Id.ToString()).ToList();
            var pagedCompanyEmails = pagedList.Select(c => c.Email).ToList();
            
            // Tüm davetli e-postaları topla
            var pagedInvitedEmails = allInvitations
                .Where(inv => pagedCompanyIds.Contains(inv.InvitedBy))
                .ToList();
            
            // Tüm ilgili e-postaları tek seferde al
            var pagedRelevantEmails = pagedCompanyEmails
                .Concat(pagedInvitedEmails.Select(i => i.Email))
                .Distinct()
                .ToList();
            
            // Tek sorguda tüm ilan sayılarını al (pagination sonrası TotalAnnouncement için)
            var pagedAnnouncementCounts = await _readAnnouncementRepository.Queryable()
                .Where(a => (a.Status == "active" || a.Status == "Active" || a.Status == "ACTIVE") && pagedRelevantEmails.Contains(a.Email))
                .GroupBy(a => a.Email)
                .Select(g => new { Email = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Email, x => x.Count, cancellationToken);

            // Her firma için ilan sayısını hesapla (bellekte)
            for (int i = 0; i < mapped.Count; i++)
            {
                var company = pagedList[i];

                // Owner + invited consultant emails for this company
                var invitedEmailsForCompany = pagedInvitedEmails
                    .Where(inv => inv.InvitedBy == company.Id.ToString())
                    .Select(inv => inv.Email)
                    .ToList();

                invitedEmailsForCompany.Add(company.Email);

                // Bellekteki sözlükten ilan sayılarını topla
                var activeCountForCompany = invitedEmailsForCompany
                    .Where(email => pagedAnnouncementCounts.ContainsKey(email))
                    .Sum(email => pagedAnnouncementCounts[email]);

                mapped[i].TotalAnnouncement = activeCountForCompany;
                mapped[i].CompanyMembershipDuration = (int)((DateTime.Now - mapped[i].RegistrationDate)?.TotalDays / 30);
            }

            var totalItems = companyList.Count;
            var totalPage = (int)Math.Ceiling((double)(totalItems > 0 ? totalItems : 1) / request.Size);

            return new ApiResponse<List<GetCompaniesQueryResponse>>
            {
                Code = 200,
                Data = mapped,
                IsSuccessfull = true,
                Message = "Şirketler başarıyla getirildi.",
                Meta = new MetaData
                {
                    Pagination = new Pagination
                    {
                        PageNumber = request.Page,
                        PageSize = request.Size,
                        TotalItem = totalItems,
                        TotalPage = totalPage
                    }
                }
            };
        }

        public async Task<ApiResponse<GetTotalCompanyCountQueryResponse>> GetTotalCompanyCountAsync(GetTotalCompanyCountQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var consultantsQuery = _readRepository.Queryable()
                 .Where(u => !u.IsConsultant &&
                           (u.UserTypes == "Emlakçı" || u.UserTypes == "Kurumsal" || u.UserTypes == "Vakıf"
                           || u.UserTypes == "İnşaat" || u.UserTypes == "Banka"));

                var totalCount = await consultantsQuery.CountAsync(cancellationToken);

                var response = new GetTotalCompanyCountQueryResponse
                {
                    TotalCount = totalCount,
                    Message = $"Toplam {totalCount} firma bulundu."
                };

                return new ApiResponse<GetTotalCompanyCountQueryResponse>
                {
                    Data = response,
                    IsSuccessfull = true,
                    Message = "Firma sayısı başarıyla getirildi.",
                    Code = 200
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<GetTotalCompanyCountQueryResponse>
                {
                    Data = null,
                    IsSuccessfull = false,
                    Message = $"Firma sayısı getirilirken hata oluştu: {ex.Message}",
                    Code = 500
                };
            }
        }
    }
}

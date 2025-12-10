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

            // 2️⃣ Gerekli verileri tek seferde çek
            var companyList = await companiesQuery.ToListAsync(cancellationToken);
            var companyIds = companyList.Select(c => c.Id.ToString()).ToList();
            var companyEmails = companyList.Select(c => c.Email).ToList();

            var allInvitations = await _consultantInvitationRepository.Queryable()
                .Where(ci => companyIds.Contains(ci.InvitedBy))
                .ToListAsync(cancellationToken);

            var allAnnouncements = await _readAnnouncementRepository.Queryable()
                .Where(a => a.Status == "active" &&
                            (companyEmails.Contains(a.Email) ||
                             allInvitations.Select(i => i.Email).Contains(a.Email)))
                .ToListAsync(cancellationToken);

            // 3️⃣ Bellekte lookup hazırla
            var invitationsLookup = allInvitations
                .GroupBy(i => i.InvitedBy)
                .ToDictionary(g => g.Key, g => g.Select(i => i.Email).ToList());

            var announcementsLookup = allAnnouncements
                .GroupBy(a => a.Email)
                .ToDictionary(g => g.Key, g => g.Count());

            // 4️⃣ Toplam ilan sayısını hesapla
            var companyAnnouncementCounts = companyList.ToDictionary(c => c.Id.ToString(), c =>
            {
                int count = 0;

                // Firma kendi ilanları
                if (announcementsLookup.ContainsKey(c.Email))
                    count += announcementsLookup[c.Email];

                // Davetli ilanları
                if (invitationsLookup.ContainsKey(c.Id.ToString()))
                {
                    foreach (var invitedEmail in invitationsLookup[c.Id.ToString()])
                        if (announcementsLookup.ContainsKey(invitedEmail))
                            count += announcementsLookup[invitedEmail];
                }

                return count;
            });

            // 5️⃣ Sıralama
            companyList = request.SortBy switch
            {
                "company_asc" => companyList.OrderBy(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "company_desc" => companyList.OrderByDescending(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "announcement_asc" => companyList.OrderBy(c => companyAnnouncementCounts[c.Id.ToString()])
                                                 .ThenBy(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "announcement_desc" => companyList.OrderByDescending(c => companyAnnouncementCounts[c.Id.ToString()])
                                                  .ThenBy(c => c.CompanyName, StringComparer.Create(new CultureInfo("tr-TR"), true)).ToList(),
                "date_asc" => companyList.OrderBy(c => c.RegistrationDate).ToList(),
                "date_desc" => companyList.OrderByDescending(c => c.RegistrationDate).ToList(),
                _ => companyList.OrderByDescending(c => c.RegistrationDate).ToList()
            };

            // 6️⃣ Pagination
            var pagedList = companyList
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .ToList();

            var mapped = _mapper.Map<List<GetCompaniesQueryResponse>>(pagedList);

            // Recalculate counts for the current page using the exact same logic as detail/AboutUs (active only)
            for (int i = 0; i < mapped.Count; i++)
            {
                var company = pagedList[i];

                // Owner + invited consultant emails for this company
                var invitedEmailsForCompany = allInvitations
                    .Where(inv => inv.InvitedBy == company.Id.ToString())
                    .Select(inv => inv.Email)
                    .ToList();

                invitedEmailsForCompany.Add(company.Email);

                // Count active announcements for those emails
                var activeCountForCompany = await _readAnnouncementRepository.Queryable()
                    .Where(a => a.Status == "active" && invitedEmailsForCompany.Contains(a.Email))
                    .CountAsync(cancellationToken);

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

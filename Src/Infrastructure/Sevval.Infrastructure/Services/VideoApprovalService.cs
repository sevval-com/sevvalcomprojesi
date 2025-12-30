using Microsoft.EntityFrameworkCore;
using Sevval.Application.Interfaces.Services;
using Sevval.Domain.Enums;
using Sevval.Persistence.Context;
using Sevval.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sevval.Infrastructure.Services
{
    /// <summary>
    /// Video onay işlemlerini yöneten servis
    /// </summary>
    public class VideoApprovalService : IVideoApprovalService
    {
        private readonly ApplicationDbContext _context;
        private static readonly string[] SuperAdminEmails = new[] { "sftumen41@gmail.com" };

        public VideoApprovalService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<List<VideolarSayfasi>> GetPendingVideosAsync()
        {
            return await _context.VideolarSayfasi
                .Include(v => v.YukleyenKullanici)
                .Where(v => v.ApprovalStatus == VideoApprovalStatus.Pending)
                .OrderByDescending(v => v.YuklenmeTarihi)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetPendingVideoCountAsync()
        {
            return await _context.VideolarSayfasi
                .CountAsync(v => v.ApprovalStatus == VideoApprovalStatus.Pending);
        }

        /// <inheritdoc/>
        public async Task<bool> ApproveVideoAsync(int videoId, string approverUserId)
        {
            var video = await _context.VideolarSayfasi.FindAsync(videoId);
            if (video == null)
                return false;

            // Sadece Pending durumundaki videolar onaylanabilir
            if (video.ApprovalStatus != VideoApprovalStatus.Pending)
                return false;

            video.ApprovalStatus = VideoApprovalStatus.Approved;
            video.ApprovalDate = DateTime.UtcNow;
            video.ApprovedByUserId = approverUserId;
            video.RejectionReason = null;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> RejectVideoAsync(int videoId, string approverUserId, string? reason)
        {
            var video = await _context.VideolarSayfasi.FindAsync(videoId);
            if (video == null)
                return false;

            // Sadece Pending durumundaki videolar reddedilebilir
            if (video.ApprovalStatus != VideoApprovalStatus.Pending)
                return false;

            video.ApprovalStatus = VideoApprovalStatus.Rejected;
            video.ApprovalDate = DateTime.UtcNow;
            video.ApprovedByUserId = approverUserId;
            video.RejectionReason = reason;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public bool IsSuperAdmin(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return SuperAdminEmails.Any(e => string.Equals(email, e, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        public async Task<List<VideolarSayfasi>> GetUserVideosByStatusAsync(string userId, VideoApprovalStatus? status)
        {
            var query = _context.VideolarSayfasi
                .Where(v => v.YukleyenKullaniciId == userId);

            if (status.HasValue)
            {
                query = query.Where(v => v.ApprovalStatus == status.Value);
            }

            return await query
                .OrderByDescending(v => v.YuklenmeTarihi)
                .ToListAsync();
        }
    }
}

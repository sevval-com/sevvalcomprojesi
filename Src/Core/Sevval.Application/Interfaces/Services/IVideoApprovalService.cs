using Sevval.Domain.Enums;
using Sevval.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.Services
{
    /// <summary>
    /// Video onay işlemlerini yöneten servis interface'i
    /// </summary>
    public interface IVideoApprovalService
    {
        /// <summary>
        /// Admin paneli için bekleyen videoları getirir
        /// </summary>
        Task<List<VideolarSayfasi>> GetPendingVideosAsync();

        /// <summary>
        /// Bekleyen video sayısını getirir (badge için)
        /// </summary>
        Task<int> GetPendingVideoCountAsync();

        /// <summary>
        /// Videoyu onaylar
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <param name="approverUserId">Onaylayan kullanıcı ID</param>
        /// <returns>Başarılı ise true</returns>
        Task<bool> ApproveVideoAsync(int videoId, string approverUserId);

        /// <summary>
        /// Videoyu reddeder
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <param name="approverUserId">Reddeden kullanıcı ID</param>
        /// <param name="reason">Red nedeni (opsiyonel)</param>
        /// <returns>Başarılı ise true</returns>
        Task<bool> RejectVideoAsync(int videoId, string approverUserId, string? reason);

        /// <summary>
        /// Kullanıcının Super Admin olup olmadığını kontrol eder
        /// </summary>
        /// <param name="email">Kullanıcı email adresi</param>
        /// <returns>Super Admin ise true</returns>
        bool IsSuperAdmin(string email);

        /// <summary>
        /// Kullanıcının videolarını onay durumuna göre getirir
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <param name="status">Onay durumu (null ise tümü)</param>
        Task<List<VideolarSayfasi>> GetUserVideosByStatusAsync(string userId, VideoApprovalStatus? status);
    }
}

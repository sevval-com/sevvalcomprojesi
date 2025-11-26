using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;

namespace sevvalemlak.csproj.Services
{
    /// <summary>
    /// Background service that permanently deletes user accounts 
    /// that have been marked as deleted for more than 30 days.
    /// Runs daily at 2:00 AM.
    /// </summary>
    public class AccountCleanupService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<AccountCleanupService> _logger;
        private Timer? _timer;

        public AccountCleanupService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<AccountCleanupService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Account Cleanup Service started.");

            // Calculate next 2:00 AM
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var firstRunDelay = nextRun - now;
            _logger.LogInformation($"First cleanup will run at {nextRun}");

            // Run daily at 2:00 AM
            _timer = new Timer(
                CleanupExpiredAccounts,
                null,
                firstRunDelay,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private async void CleanupExpiredAccounts(object? state)
        {
            _logger.LogInformation("Starting account cleanup task...");

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Find accounts deleted more than 30 days ago
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var expiredAccounts = await context.DeletedAccounts
                    .Where(d => d.DeletedAt < thirtyDaysAgo)
                    .ToListAsync();

                _logger.LogInformation($"Found {expiredAccounts.Count} expired accounts to delete.");

                foreach (var deletedAccount in expiredAccounts)
                {
                    try
                    {
                        var user = await userManager.FindByIdAsync(deletedAccount.UserId);
                        if (user != null)
                        {
                            // CASCADE DELETE: Remove all related data
                            await DeleteUserRelatedDataAsync(context, user.Email, user.Id);

                            // HARD DELETE: Permanently remove user from Identity system
                            var result = await userManager.DeleteAsync(user);
                            
                            if (result.Succeeded)
                            {
                                // Remove from DeletedAccounts table
                                context.DeletedAccounts.Remove(deletedAccount);
                                await context.SaveChangesAsync();
                                
                                _logger.LogInformation($"Permanently deleted user: {user.Email} (ID: {user.Id})");
                            }
                            else
                            {
                                _logger.LogError($"Failed to delete user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                            }
                        }
                        else
                        {
                            // User already deleted, just clean up tracking record
                            context.DeletedAccounts.Remove(deletedAccount);
                            await context.SaveChangesAsync();
                            _logger.LogWarning($"User not found for DeletedAccount ID {deletedAccount.Id}, removing tracking record.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error deleting account {deletedAccount.UserId}");
                    }
                }

                _logger.LogInformation("Account cleanup task completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during account cleanup task.");
            }
        }

        /// <summary>
        /// Cascade delete all user-related data from database
        /// </summary>
        private async Task DeleteUserRelatedDataAsync(ApplicationDbContext context, string userEmail, string userId)
        {
            try
            {
                // Delete IlanBilgileri (user's property listings)
                var ilanlar = await context.IlanBilgileri
                    .Where(i => i.Email == userEmail)
                    .ToListAsync();
                context.IlanBilgileri.RemoveRange(ilanlar);

                // Delete Messages (sent by user)
                var messages = await context.Messages
                    .Where(m => m.SenderEmail == userEmail)
                    .ToListAsync();
                context.Messages.RemoveRange(messages);

                // Delete Comments
                var comments = await context.Comments
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
                context.Comments.RemoveRange(comments);

                // Delete UserRefreshTokens
                var refreshTokens = await context.UserRefreshTokens
                    .Where(t => t.UserId == userId)
                    .ToListAsync();
                context.UserRefreshTokens.RemoveRange(refreshTokens);

                // Delete RecentlyVisitedAnnouncements
                var recentVisits = await context.RecentlyVisitedAnnouncements
                    .Where(r => r.UserId == userId)
                    .ToListAsync();
                context.RecentlyVisitedAnnouncements.RemoveRange(recentVisits);

                // Add other related entities as needed
                // Example: BireyselIlanTakipleri, Sepet, etc.

                await context.SaveChangesAsync();
                _logger.LogInformation($"Deleted all related data for user {userEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting related data for user {userEmail}");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Account Cleanup Service stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

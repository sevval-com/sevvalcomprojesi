using Microsoft.EntityFrameworkCore;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;

namespace sevvalemlak.Services
{
    public class VisitorService
    {
        private readonly ApplicationDbContext _context;

        public VisitorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalVisitorCountAsync()
        {
            return await _context.Visitors.CountAsync();
        }

        public async Task<int> GetActiveVisitorCountAsync()
        {
            var activeVisitorCount = await _context.Visitors
                .Where(v => v.VisitTime >= DateTime.Now.AddMinutes(-5)) // Son 5 dakikada ziyaret edenler
                .CountAsync();
            return activeVisitorCount;
        }

        public async Task AddVisitorAsync(string ipAddress)
        {
            var visitor = new Visitor
            {
                IpAddress = ipAddress,
                VisitTime = DateTime.Now
            };
            _context.Visitors.Add(visitor);
            await _context.SaveChangesAsync();
        }

        // Yeni metotlar ekleniyor
        public Task RecordVisit(string ipAddress)
        {
            return AddVisitorAsync(ipAddress);
        }

        public Task<int> GetTotalVisitorCount()
        {
            return GetTotalVisitorCountAsync();
        }

        public Task<int> GetActiveVisitorCount()
        {
            return GetActiveVisitorCountAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Util;
using VisitCountApi.Data;
using VisitCountApi.Entities;
using VisitCountApi.Services.Interfaces;

namespace VisitCountApi.Services
{
    public class VisitService : IVisitService
    {
        private readonly VisitorCountContext _context;
        private readonly ILogger<VisitService> _logger;
        private readonly int _expirationTime;
        public VisitService(IConfiguration configuration, VisitorCountContext context, ILogger<VisitService> logger)
        {
            _context = context;
            _logger = logger;
            _expirationTime = configuration.GetValue<int>("CookieSettings:ExpirationTime");
        }
        public async Task<bool> AddVisitorAsync(Guid visitID)
        {
            try
            {
                if (await _context.Visitors.AnyAsync(v => v.VisitId == visitID))
                {
                    visitID = Guid.NewGuid();
                }
                var visitor = new Visitor
                {
                    VisitId = visitID,
                    InstertDateTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now,
                    ExpiryDateTime = DateTime.Now.AddMinutes(_expirationTime),
                    VisitCount = 1
                };
                _context.Visitors.Add(visitor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VisitService AddVisitorAsync");
                return false;
            }
        }

        public async Task<bool> UpdateVisitorAsync(Guid visitID)
        {
            try
            {
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.VisitId == visitID && v.ExpiryDateTime >= DateTime.Now.AddMinutes(-_expirationTime));
                if (visitor is null)
                {
                    _logger.LogWarning("Visitor not found for VisitID: {VisitID}", visitID);
                    return false;
                }
                await _context.Visitors.ExecuteUpdateAsync(setter => setter
                        .SetProperty(p => p.LastUpdateDateTime, DateTime.Now)
                        .SetProperty(p => p.VisitCount, p => p.VisitCount + 1)

                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VisitService UpdateVisitorAsync");
                return false;
            }
        }


        public async Task<bool> UpdateDailyVisitAsync()
        {
            try
            {
                int today = PersianDateTime.Now.ToInt();
                if (await _context.DailyVisits.AnyAsync(d => d.PersianDateID == today))
                {
                    await _context.DailyVisits.ExecuteUpdateAsync(setter => setter
                        .SetProperty(p => p.TotalVisits, p => p.TotalVisits + 1)
                    );
                }
                else
                {
                    var dailyVisit = new DailyVisit
                    {
                        PersianDateID = today,
                        InsertDateTime = DateTime.Now,
                        TotalVisits = 1
                    };
                    _context.DailyVisits.Add(dailyVisit);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VisitService UpdateDailyVisitAsync");
                return false ;
            }
          

        }
    }
}

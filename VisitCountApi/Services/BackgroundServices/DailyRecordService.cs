
using Microsoft.EntityFrameworkCore;
using Util;
using VisitCountApi.Data;
using VisitCountApi.Entities;

namespace VisitCountApi.Services.BackgroundServices
{
    public class DailyRecordService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyRecordService> _logger;
        public DailyRecordService(IServiceProvider serviceProvider, ILogger<DailyRecordService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<VisitorCountContext>();

                    int todayPersianDateId = PersianDateTime.Now.ToInt();

                    var exists = await context.DailyVisits.AnyAsync(d => d.PersianDateID == todayPersianDateId, stoppingToken);
                    if (!exists)
                    {
                        var dailyVisit = new DailyVisit
                        {
                            PersianDateID = todayPersianDateId,
                            InsertDateTime = DateTime.Now,
                            TotalVisits = 0
                        };
                        context.DailyVisits.Add(dailyVisit);
                        await context.SaveChangesAsync(stoppingToken);
                    }

                    // Calculate time until next midnight
                    var now = DateTime.Now;
                    var nextRunTime = now.Date.AddDays(1);
                    var delay = nextRunTime - now;


                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "DailyRecordService");
                    throw;
                }
            
            
            }
        }
    }
}

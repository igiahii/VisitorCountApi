
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VisitCountApi.Data;

namespace VisitCountApi.Services.BackgroundServices
{
    public class DeleteExpiryRecordService : BackgroundService
    {
        private readonly ILogger<DeleteExpiryRecordService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _expiryCookieTime;
        public DeleteExpiryRecordService(ILogger<DeleteExpiryRecordService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _expiryCookieTime = configuration.GetValue<int>("CookieSettings:ExpirationTime");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<VisitorCountContext>();
                    DateTime expiryThreshold = DateTime.Now.AddMinutes(-_expiryCookieTime);
                    await context.Visitors.Where(v => v.ExpiryDateTime.HasValue && v.ExpiryDateTime < expiryThreshold).ExecuteDeleteAsync(stoppingToken);

                    //calculate delay for 30 seconds
                    var now = DateTime.Now;
                    var nextRunTime = now.AddSeconds(30);
                    var delay = nextRunTime - now;

                    await Task.Delay(delay, stoppingToken);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "DeleteExpiryRecordService");
                    throw;
                }

            }
        }
    }
}

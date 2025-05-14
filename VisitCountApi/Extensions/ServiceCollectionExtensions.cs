using VisitCountApi.Services.BackgroundServices;
using VisitCountApi.Services.Interfaces;
using VisitCountApi.Services;

namespace VisitCountApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices (this IServiceCollection services)
        {
            services.AddScoped<IVisitService, VisitService>();
            //Background Services
            services.AddHostedService<DailyRecordService>();
            services.AddHostedService<DeleteExpiryRecordService>();

            return services;
        }
    }
}

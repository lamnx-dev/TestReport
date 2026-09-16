using Microsoft.Extensions.DependencyInjection;
using TestReport.Api.Application.Interfaces;
using TestReport.Api.Application.Services;

namespace TestReport.Api.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMultiColumnReportService, MultiColumnReportService>();
            return services;
        }
    }
}


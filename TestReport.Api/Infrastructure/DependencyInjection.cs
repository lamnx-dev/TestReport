using Microsoft.Extensions.DependencyInjection;
using TestReport.Api.Application.Interfaces;
using TestReport.Api.Infrastructure.DbMappings;
using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Repositories;
using TestReport.Api.Infrastructure.SelectProviders;

namespace TestReport.Api.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Database Query & Mapping Providers (Oracle)
            services.AddSingleton<MultiColumnSelectProvider>();
            services.AddSingleton<MultiColumnDbMapping>();

            // Repositories
            services.AddScoped<IMultiColumnReportRepository, MultiColumnReportRepository>();

            // Report Renderers (DevExpress)
            services.AddScoped<IReportRenderer, DevExpressMultiColumnReportRenderer>();

            return services;
        }
    }
}


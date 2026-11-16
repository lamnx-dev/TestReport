using TestReport.Api.Infrastructure.Renderers;
using TestReport.Api.Infrastructure.Reports;

namespace TestReport.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<SampleReportFactory>();
        services.AddScoped<ReportPdfRenderer>();
        return services;
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Framework.SoftDelete;

public static class SoftDeleteServiceCollectionExtensions
{
    public static IServiceCollection AddSoftDeleteCleanup(
        this IServiceCollection services,
        IConfiguration configuration,
        SoftDeleteCleanupPlan plan,
        string sectionName = "SoftDeleteCleanup")
    {
        services.Configure<SoftDeleteCleanupOptions>(configuration.GetSection(sectionName));
        services.AddSingleton(plan);
        services.AddHostedService<SoftDeleteCleanupService>();

        return services;
    }
}

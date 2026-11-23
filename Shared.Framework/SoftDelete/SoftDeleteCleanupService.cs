using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Framework.Data;

namespace Shared.Framework.SoftDelete;

public sealed class SoftDeleteCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<SoftDeleteCleanupOptions> _options;
    private readonly SoftDeleteCleanupPlan _plan;
    private readonly ILogger<SoftDeleteCleanupService> _logger;

    public SoftDeleteCleanupService(
        IServiceScopeFactory scopeFactory,
        IOptions<SoftDeleteCleanupOptions> options,
        SoftDeleteCleanupPlan plan,
        ILogger<SoftDeleteCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _plan = plan;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SoftDeleteCleanupOptions options = _options.Value;
        if (!options.Enabled)
        {
            _logger.LogInformation("Soft delete cleanup is disabled");
            return;
        }

        if (options.RetentionDays <= 0 || options.IntervalMinutes <= 0)
        {
            _logger.LogError(
                "Soft delete cleanup is misconfigured: RetentionDays={RetentionDays}, IntervalMinutes={IntervalMinutes}",
                options.RetentionDays,
                options.IntervalMinutes);
            return;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.IntervalMinutes));

        do
        {
            await CleanupAsync(options.RetentionDays, stoppingToken).ConfigureAwait(false);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }

    private async Task CleanupAsync(int retentionDays, CancellationToken cancellationToken)
    {
        DateTime cutoff = DateTime.UtcNow.AddDays(-retentionDays);
        _logger.LogInformation("Starting soft delete cleanup with cutoff {Cutoff}", cutoff);

        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            IDbConnectionFactory connectionFactory = scope.ServiceProvider
                .GetRequiredService<IDbConnectionFactory>();

            using IDbConnection connection = await connectionFactory
                .CreateConnectionAsync(cancellationToken)
                .ConfigureAwait(false);

            using IDbTransaction transaction = connection.BeginTransaction();

            var parameters = new { cutoff };
            var results = new List<string>(_plan.Steps.Count);

            foreach (SoftDeleteCleanupStep step in _plan.Steps)
            {
                int deleted = await connection
                    .ExecuteAsync(step.Sql, parameters, transaction)
                    .ConfigureAwait(false);

                results.Add($"{step.Name}={deleted}");
            }

            transaction.Commit();

            _logger.LogInformation(
                "Soft delete cleanup removed {Summary}",
                string.Join(", ", results));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Soft delete cleanup failed");
        }
    }
}

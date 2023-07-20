using IdGen;
using Microsoft.EntityFrameworkCore;
using Wobalization.Entities.DatabaseContexts;

namespace Wobalization.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdGenerator(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
        {
            var structure = new IdStructure(45, 6, 12);
            var epoch = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var options = new IdGeneratorOptions(
                structure,
                new DefaultTimeSource(epoch),
                SequenceOverflowStrategy.SpinWait);

            return new IdGenerator(0, options);
        });

        return services;
    }

    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddDbContextPool<DatabaseContext>(options =>
            options
                .UseSqlite(configuration.GetConnectionString("DatabaseContext"))
                .EnableSensitiveDataLogging(!environment.IsProduction())
                .EnableDetailedErrors(!environment.IsProduction())
                .UseSnakeCaseNamingConvention()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );

        return services;
    }
}
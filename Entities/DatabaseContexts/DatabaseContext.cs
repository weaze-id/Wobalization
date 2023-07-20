using Microsoft.EntityFrameworkCore;
using Wobalization.Entities.Configurations;

namespace Wobalization.Entities.DatabaseContexts;

public class DatabaseContext : DbContext
{
    public const int PaginationSize = 25;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<App>? App { get; set; }
    public DbSet<Language>? Language { get; set; }
    public DbSet<TranslationKey>? TranslationKey { get; set; }
    public DbSet<TranslationValue>? TranslationValue { get; set; }
    public DbSet<User>? User { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AppConfiguration());
        builder.ApplyConfiguration(new LanguageConfiguration());
        builder.ApplyConfiguration(new TranslationKeyConfiguration());
        builder.ApplyConfiguration(new TranslationValueConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }
}
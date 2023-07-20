using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wobalization.Database.Models;

namespace Wobalization.Database.Configurations;

public class AppConfiguration : IEntityTypeConfiguration<App>
{
    public void Configure(EntityTypeBuilder<App> builder)
    {
        builder
            .Property(e => e.Id)
            .IsRequired();

        builder
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(e => e.Key)
            .IsRequired();

        builder
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
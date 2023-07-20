using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wobalization.Database.Models;

namespace Wobalization.Database.Configurations;

public class TranslationLanguageConfiguration : IEntityTypeConfiguration<TranslationLanguage>
{
    public void Configure(EntityTypeBuilder<TranslationLanguage> builder)
    {
        builder
            .Property(e => e.Id)
            .IsRequired();

        builder
            .Property(e => e.AppId)
            .IsRequired();

        builder
            .Property(e => e.Culture)
            .IsRequired()
            .HasMaxLength(10);

        builder
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
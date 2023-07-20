using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wobalization.Entities.Configurations;

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
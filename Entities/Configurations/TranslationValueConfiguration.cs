using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wobalization.Entities.Configurations;

public class TranslationValueConfiguration : IEntityTypeConfiguration<TranslationValue>
{
    public void Configure(EntityTypeBuilder<TranslationValue> builder)
    {
        builder
            .Property(e => e.Id)
            .IsRequired();

        builder
            .Property(e => e.LanguageId)
            .IsRequired();

        builder
            .Property(e => e.Value)
            .IsRequired();

        builder
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wobalization.Database.Models;

namespace Wobalization.Database.Configurations;

public class TranslationValueConfiguration : IEntityTypeConfiguration<TranslationValue>
{
    public void Configure(EntityTypeBuilder<TranslationValue> builder)
    {
        builder
            .Property(e => e.Id)
            .IsRequired();

        builder
            .Property(e => e.TranslationLanguageId)
            .IsRequired();

        builder
            .Property(e => e.Value)
            .IsRequired();

        builder
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wobalization.Entities.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(e => e.Id)
            .IsRequired();

        builder
            .Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(60)
            .IsFixedLength();

        builder
            .Property(e => e.CreatedAt)
            .IsRequired();
    }
}
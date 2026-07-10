using IAS.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Identity.Infrastructure.Congufirations;

public class RefreshTokenConfigrtation : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(rt => rt.ExpiresOnUTC)
            .IsRequired();

        builder.HasIndex(rt => rt.Token)
            .IsUnique();
        builder.HasOne(builder => builder.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
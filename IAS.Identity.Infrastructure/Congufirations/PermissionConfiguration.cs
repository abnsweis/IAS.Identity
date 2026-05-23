using IAS.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Identity.Infrastructure.Congufirations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(u => u.Id).IsUnique();
        builder.HasIndex(u => u.Name).IsUnique();
        builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        builder.HasMany(p => p.RolePermissions)
           .WithOne(rp => rp.Permission)
           .HasForeignKey(rp => rp.PermissionId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
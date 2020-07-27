using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestApi.Domain;

namespace TestApi.DAL.Mappings
{
    public class AllocationMap : IEntityTypeConfiguration<Allocation>
    {
        public void Configure(EntityTypeBuilder<Allocation> builder)
        {
            builder.ToTable(nameof(Allocation));
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.UserId }).IsUnique();
            builder.Property(x => x.Username);
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.Allocated);
        }
    }
}

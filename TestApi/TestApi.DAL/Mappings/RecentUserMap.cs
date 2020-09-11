using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestApi.Domain;

namespace TestApi.DAL.Mappings
{
    public class RecentUserMap : IEntityTypeConfiguration<RecentUser>
    {
        public void Configure(EntityTypeBuilder<RecentUser> builder)
        {
            builder.ToTable(nameof(RecentUser));
            builder.HasIndex(x => x.Username).IsUnique();
            builder.Property(x => x.CreatedAt);
        }
    }
}

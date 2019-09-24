using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Publisher.Model
{
    public class PingConfiguration : IEntityTypeConfiguration<Ping>
    {
        public void Configure(EntityTypeBuilder<Ping> builder)
        {
            builder.ToTable("Pings");
            builder.Property(c => c.Value).IsRequired();
        }
    }
}
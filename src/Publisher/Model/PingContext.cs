using Microsoft.EntityFrameworkCore;

namespace Publisher.Model
{
    public class PingContext: DbContext
    {
        public PingContext(DbContextOptions<PingContext> options) : base(options)
        {

        }

        public DbSet<Ping> Pings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PingConfiguration());            

            base.OnModelCreating(modelBuilder);
        }
    }
}
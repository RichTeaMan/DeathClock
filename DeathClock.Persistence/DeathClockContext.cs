using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DeathClock.Persistence
{

    public class DeathClockContext : DbContext
    {
        public DbSet<TmdbPerson> TmdbPersons { get; set; }

        public DeathClockContext(DbContextOptions<DeathClockContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TmdbPerson>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();

            var connectionString = config.GetConnectionString("DeathClockDatabase");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}

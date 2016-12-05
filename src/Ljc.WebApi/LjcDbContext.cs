using Ljc.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ljc.WebApi
{
    public class LjcDbContext : DbContext
    {
        public static string DbConnectionString;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             => optionsBuilder
                 .UseMySql(DbConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimeStatistic>();
            modelBuilder.Entity<MyUser>();
        }

        public virtual DbSet<TimeStatistic> Timestatistic { get; set; }
        public virtual DbSet<MyUser> Myuser { get; set; }
    }
}

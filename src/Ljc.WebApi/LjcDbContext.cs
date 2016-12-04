using Ljc.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ljc.WebApi
{
    public class LjcDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
             => optionsBuilder
                 .UseMySql(@"server=localhost;database=dev;uid=root;pwd=000000;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimeStatistic>();
            modelBuilder.Entity<MyUser>();
        }

        public virtual DbSet<TimeStatistic> Timestatistic { get; set; }
        public virtual DbSet<MyUser> Myuser { get; set; }
    }
}

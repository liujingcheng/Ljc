using Ljc.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ljc.WebApi
{
    public class LjcDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(DbHelper.DbConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimeStatistic>();
            modelBuilder.Entity<MyUser>();
        }

        public virtual DbSet<TimeStatistic> timestatistic { get; set; }
        public virtual DbSet<MyUser> myuser { get; set; }
    }
}

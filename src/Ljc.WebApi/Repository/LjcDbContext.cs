using Ljc.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ljc.WebApi.Repository
{
    /// <summary>
    /// The entity framework context with XXX DbSet
    /// </summary>
    public class LjcDbContext : DbContext
    {
        public LjcDbContext(DbContextOptions<LjcDbContext> options)
        : base(options)
        { }

        public DbSet<TimeStatistic> TimeStatisticDbSet { get; set; }
    }
}

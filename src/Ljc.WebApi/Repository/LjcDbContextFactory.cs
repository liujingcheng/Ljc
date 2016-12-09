using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace Ljc.WebApi.Repository
{
    /// <summary>
    /// Factory class for LjcDbContext
    /// </summary>
    public static class LjcDbContextFactory
    {
        public static string DbConnectionString;
        public static LjcDbContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<LjcDbContext>();
            optionsBuilder.UseMySQL(DbConnectionString);

            //Ensure database creation
            var context = new LjcDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}

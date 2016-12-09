using Microsoft.EntityFrameworkCore;

namespace Ljc.WebApi
{
    public static class DbHelper
    {
        public static string DbConnectionString;
        public static DbContextOptionsBuilder Builder = new DbContextOptionsBuilder();
    }
}

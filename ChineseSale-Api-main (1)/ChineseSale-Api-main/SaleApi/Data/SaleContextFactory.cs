
using Microsoft.EntityFrameworkCore;

namespace SaleApi.Data
{
    public class SaleContextFactory
    {
        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SaleDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

        public static SaleContextDB CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SaleContextDB>();
            optionsBuilder.UseSqlServer(ConnectionString);
            return new SaleContextDB(optionsBuilder.Options);
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace WebApplication.Models
{
    public class ConnectionDB: DbContext
    {
        public DbSet<Car> Car { get; set; }
        public DbSet<Dispatch> Dispatch { get; set; }

        public ConnectionDB(DbContextOptions options) : base(options)
        {
        }

    }
}

using Microsoft.EntityFrameworkCore;
using Web2Project.Models;

namespace Web2Project.Repository
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        public DbSet<Korisnik> Korisnik { get; set; }
    }
}

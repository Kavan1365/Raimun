using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.DataLayer
{
    public class RaimunDbContext : DbContext
    {
        public RaimunDbContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Weather> Weathers { get; set; }
    }
}

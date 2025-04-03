
using Microsoft.EntityFrameworkCore;
using ShitpostTron5000.Data;

namespace ShitpostTron5000
{
    public class ShitpostTronContext : DbContext
    {
        public ShitpostTronContext(DbContextOptions<ShitpostTronContext> context) : base(context)
        {
        
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=shitposttronDB.dat");
        }

        public ShitpostTronContext()
        {
        }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Timer> Timers { get; set; }
    }
}

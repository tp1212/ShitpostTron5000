
using Microsoft.EntityFrameworkCore;
using ShitpostTron5000.Data;

namespace ShitpostTron5000
{
    class ShitpostTronContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=ShitPostTron;Integrated Security=True;");
            base.OnConfiguring(optionsBuilder);
        }

        public ShitpostTronContext()
        {
        }

        public DbSet<Quote> Quotes { get; set; }

    }
}

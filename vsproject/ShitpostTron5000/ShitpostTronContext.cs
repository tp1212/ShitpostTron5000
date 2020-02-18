
using Microsoft.EntityFrameworkCore;
using ShitpostTron5000.Data;

namespace ShitpostTron5000
{
    public class ShitpostTronContext : DbContext
    {


        public ShitpostTronContext(DbContextOptions<ShitpostTronContext> context) :base(context)
        {
        
        }


        public ShitpostTronContext()
        {
        }

        public DbSet<Quote> Quotes { get; set; }

    }
}

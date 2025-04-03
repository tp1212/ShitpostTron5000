using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShitpostTron5000
{


    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<ShitpostTronContext>
    {
        public ShitpostTronContext CreateDbContext(string[] args)
        {
            var option = new DbContextOptionsBuilder<ShitpostTronContext>();
                option.UseSqlite("Data Source=shitposttronDB.dat");              
            return new ShitpostTronContext(option.Options);
        }
    }
}

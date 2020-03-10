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
            var option = new DbContextOptionsBuilder<ShitpostTronContext>()
                .UseSqlServer(@"Server=tcp:shitposttron5000dbserver.database.windows.net,1433;Initial Catalog=ShitpostTron5000_db;Persist Security Info=False;User ID=ShitpostTronHimself;Password=Shitposttrons Super Secure Password!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;").Options;
            return new ShitpostTronContext(option);
        }
    }
}

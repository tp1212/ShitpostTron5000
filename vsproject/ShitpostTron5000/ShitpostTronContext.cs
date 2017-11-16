using System.Data.Entity;
using MySql.Data.Entity;

namespace ShitpostTron5000
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    class ShitpostTronContext : DbContext
    {
        public ShitpostTronContext() 
        {
            
        }
        public DbSet<ExpanderChannel> ExpanderChannels { get; set; }
        public DbSet<DbTest> Tests { get; set; }

        public DbSet<MessageArchiveEntry> MessageArchive { get; set; }

    }

     class DbTest
     {
         public int DbTestId { get; set; }
         public string Name { get; set; }
         public string Life { get; set; }
    }
}

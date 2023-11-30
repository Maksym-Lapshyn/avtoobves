using Microsoft.EntityFrameworkCore;

namespace Avtoobves.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
    }
}
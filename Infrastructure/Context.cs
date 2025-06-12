using Microsoft.EntityFrameworkCore;
using Avtoobves.Models;

namespace Avtoobves.Infrastructure
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        
        public virtual DbSet<BlogPost> BlogPosts { get; set; }
    }
}
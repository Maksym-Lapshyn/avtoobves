using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Avtoobves.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options)
            : base(options)
        {
            // Database.EnsureCreated();
        }
        
        // private readonly IConfiguration Configuration;
        //
        // public ProjectContext(IConfiguration configuration)
        // {
        //     Configuration = configuration;
        // }
        //
        // protected override void OnConfiguring(DbContextOptionsBuilder options)
        // {
        //     options.UseSqlServer(Configuration.GetConnectionString("AvtoobvesDatabase"));
        // }
        
        public virtual DbSet<Product> Products { get; set; }
    }

    public enum Category
    {
        Front,
        Rear,
        Footer,
        Other
    }
}
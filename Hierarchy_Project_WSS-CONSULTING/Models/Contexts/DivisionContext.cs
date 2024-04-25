using Microsoft.EntityFrameworkCore;

namespace Hierarchy_Project_WSS_CONSULTING.Models.DB
{
    public class DivisionContext : DbContext
    {
        public DbSet<Division> Divisions { get; set; }

        public DivisionContext(DbContextOptions<DivisionContext> options)
            : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}

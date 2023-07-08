using ASPDOTNET_MVC_CRUD.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ASPDOTNET_MVC_CRUD.Data
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}

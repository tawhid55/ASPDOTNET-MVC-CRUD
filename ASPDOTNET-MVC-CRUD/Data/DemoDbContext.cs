using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ASPDOTNET_MVC_CRUD.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace ASPDOTNET_MVC_CRUD.Data
{
    public class DemoDbContext : IdentityDbContext<IdentityUser>
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}

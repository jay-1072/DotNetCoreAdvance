using DotNetCoreAdvance.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreAdvance.DataContext
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Audit> AuditLogger { get; set; }
    }
}
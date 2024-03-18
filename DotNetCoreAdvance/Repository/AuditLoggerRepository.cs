using DotNetCoreAdvance.DataContext;
using DotNetCoreAdvance.Interfaces;
using DotNetCoreAdvance.Models;

namespace DotNetCoreAdvance.Repository
{
    public class AuditLoggerRepository : BaseRepository<Audit>, IAuditLoggerRepository
    {
        public AuditLoggerRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
        }
    }
}

using DotNetCoreAdvance.Models;

namespace DotNetCoreAdvance.Interfaces
{
    public interface IAuditLoggerRepository : IBaseRepository<Audit>
    {
    }
}
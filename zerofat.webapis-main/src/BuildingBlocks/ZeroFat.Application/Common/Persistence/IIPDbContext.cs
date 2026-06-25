using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Common.Persistence;

public interface IIPDbContext
{
    public bool EnableAuditing { get; set; }
    public ApplicationModule Module { get; set; }
    Task<int> SaveChangesAsync(bool withAuditing = true, bool withAuditLog = true, bool withDomain = true, CancellationToken cancellationToken = default);
}

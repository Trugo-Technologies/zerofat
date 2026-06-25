using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.Users.Application.Contracts;

public interface IRefreshTokenGenerationService : ITransientService
{
    string GenerateRefreshTokenAsync(Device device);
    Task<bool> HasValidRefreshTokenAsync(Guid userPublicId, string refreshToken);
}

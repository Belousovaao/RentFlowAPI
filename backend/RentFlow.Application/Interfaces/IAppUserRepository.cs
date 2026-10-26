using RentFlow.Domain.Auth;

namespace RentFlow.Application.Interfaces;

public interface IAppUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct);
    Task<AppUser?> GetByPhoneAsync(string phone, CancellationToken ct);
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(AppUser user, CancellationToken ct);
}

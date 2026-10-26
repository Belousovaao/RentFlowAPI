using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;

namespace RentFlow.Persistance.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly RentFlowDbContext _context;

    public AppUserRepository(RentFlowDbContext context)
    {
        _context = context;
    }

    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct) =>
        _context.AppUsers.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<AppUser?> GetByPhoneAsync(string phone, CancellationToken ct) =>
        _context.AppUsers.FirstOrDefaultAsync(u => u.Phone == phone, ct);

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(AppUser user, CancellationToken ct) =>
        await _context.AppUsers.AddAsync(user, ct);
}

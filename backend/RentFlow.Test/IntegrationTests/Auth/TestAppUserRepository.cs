using RentFlow.Application.Interfaces;
using RentFlow.Domain.Auth;

namespace RentFlow.Test.IntegrationTests.Auth;

public sealed class TestAppUserRepository : IAppUserRepository
{
    private readonly Dictionary<string, AppUser> _usersByEmail =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, AppUser> _usersByPhone =
        new(StringComparer.Ordinal);

    private readonly Dictionary<Guid, AppUser> _usersById = new();

    public void Add(AppUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        _usersById[user.Id] = user;

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            _usersByEmail[user.Email] = user;
        }

        if (!string.IsNullOrWhiteSpace(user.Phone))
        {
            _usersByPhone[user.Phone] = user;
        }
    }

    public Task<AppUser?> GetByEmailAsync(
        string email,
        CancellationToken ct)
    {
        _usersByEmail.TryGetValue(
            email,
            out var user);

        return Task.FromResult(user);
    }

    public Task<AppUser?> GetByPhoneAsync(
        string phone,
        CancellationToken ct)
    {
        _usersByPhone.TryGetValue(
            phone,
            out var user);

        return Task.FromResult(user);
    }

    public Task<AppUser?> GetByIdAsync(
        Guid id,
        CancellationToken ct)
    {
        _usersById.TryGetValue(
            id,
            out var user);

        return Task.FromResult(user);
    }

    public Task AddAsync(
        AppUser user,
        CancellationToken ct)
    {
        Add(user);

        return Task.CompletedTask;
    }
}
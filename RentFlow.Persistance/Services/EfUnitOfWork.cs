using System;
using Microsoft.EntityFrameworkCore.Storage;
using RentFlow.Application.Interfaces;

namespace RentFlow.Persistance.Services;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly RentFlowDbContext _db;
    private IDbContextTransaction? _tx;
    public EfUnitOfWork(RentFlowDbContext db)
    {
        _db = db;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _tx = await _db.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx != null)
            await _tx.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx != null)
            await _tx.RollbackAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

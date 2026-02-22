using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Common;

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

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch(DbUpdateException ex) when (IsExclusionViolation (ex))
        {
            throw new BookingConflictException();
        }
    }

    private bool IsExclusionViolation(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pgEx)
            return pgEx.SqlState == PostgresErrorCodes.ExclusionViolation;
        return false;
    }
}

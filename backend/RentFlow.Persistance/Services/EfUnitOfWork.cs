using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using RentFlow.Application.Common.Exceptions;
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
        if (_tx is null)
            return;
        await _tx.CommitAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx is null)
            return;

        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            throw new UniqueConstraintViolationException(GetConstraintName(ex));
        }
        catch(DbUpdateException ex) when (IsExclusionViolation(ex))
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

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException pgEx &&
            pgEx.SqlState == PostgresErrorCodes.UniqueViolation;
    }

    private static string? GetConstraintName(DbUpdateException ex)
    {
        return (ex.InnerException as PostgresException)?.ConstraintName;
    }

    public async Task LockBookingPaymentsAsync(
        Guid bookingId,
        CancellationToken ct = default)
    {
        if (_tx is null)
            throw new InvalidOperationException("A transaction must be started before acquiring a payment lock.");

        if (bookingId == Guid.Empty)
            throw new ArgumentException("BookingId cannot be empty.");

        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({bookingId.ToString()}, 0))",
            ct);
    }

}

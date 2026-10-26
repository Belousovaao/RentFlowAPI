using Microsoft.EntityFrameworkCore;
using Moq;
using Npgsql;
using RentFlow.Application.Common.Exceptions;
using RentFlow.Domain.Common;
using RentFlow.Persistance;
using RentFlow.Persistance.Services;
using Xunit;

namespace RentFlow.Test.UnitTests.Persistance.Services;
public sealed class EfUnitOfWorkTests
{
    private sealed class TestRentFlowDbContext : RentFlowDbContext
    {
        private readonly string _constraintName;

        public TestRentFlowDbContext(DbContextOptions<RentFlowDbContext> options, string constraintName) : base(options)
        {
            _constraintName = constraintName;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new DbUpdateException(
                "An error occurred while saving the entity changes.",
                CreatePostgresException(_constraintName));
        }

        private static PostgresException CreatePostgresException(
            string constraintName)
        {
            return new PostgresException(
                messageText:
                    $"duplicate key value violates unique constraint \"{constraintName}\"",
                severity: "ERROR",
                invariantSeverity: "ERROR",
                sqlState: PostgresErrorCodes.UniqueViolation,
                detail: null,
                hint: null,
                position: 0,
                internalPosition: 0,
                internalQuery: null,
                where: null,
                schemaName: null,
                tableName: "AppUsers",
                columnName: null,
                dataTypeName: null,
                constraintName: constraintName,
                file: null,
                line: null,
                routine: null);
        }
    }
    [Fact]
    public async Task SaveChangesAsync_WhenEmailUniqueConstraintIsViolated_ThrowsUniqueConstraintViolationException()
    {
        var options = new DbContextOptionsBuilder<RentFlowDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;
        await using var db = new TestRentFlowDbContext(options, "UX_AppUsers_Email");
        var unitOfWork = new EfUnitOfWork(db);
        var exception = await Assert.ThrowsAsync<UniqueConstraintViolationException>(
                () => unitOfWork.SaveChangesAsync());

        Assert.Equal(
            "UX_AppUsers_Email",
            exception.ConstraintName);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenPhoneUniqueConstraintIsViolated_ThrowsUniqueConstraintViolationException()
    {
        var options = new DbContextOptionsBuilder<RentFlowDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
            .Options;

        await using var db = new TestRentFlowDbContext(options, "UX_AppUsers_Phone");

        var unitOfWork = new EfUnitOfWork(db);

        var exception =
            await Assert.ThrowsAsync<UniqueConstraintViolationException>(
                () => unitOfWork.SaveChangesAsync());

        Assert.Equal(
            "UX_AppUsers_Phone",
            exception.ConstraintName);
    }
}
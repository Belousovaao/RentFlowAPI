using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RentFlow.Persistance;
using RentFlow.Persistance.Services;
using RentFlow.Test.IntegrationTests.Infrastructure;

namespace RentFlow.Test.IntegrationTests.Persistance.Services;

public sealed class EfUnitOfWorkTransactionTests : IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlContainerFixture _fixture;

    public EfUnitOfWorkTransactionTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CommitAsync_CommitsTransaction()
    {
        const string tableName = "UowCommitTest";

        await using var db = CreateDbContext();

        await CreateTestTableAsync(db, tableName);

        try
        {
            var unitOfWork = new EfUnitOfWork(db);

            await unitOfWork.BeginTransactionAsync();

            await db.Database.ExecuteSqlRawAsync($"""
                INSERT INTO "{tableName}" ("Id")
                VALUES (1)
                """);

            await unitOfWork.CommitAsync();

            await using var verificationDb = CreateDbContext();

            var count = await verificationDb.Database
                .SqlQueryRaw<int>($"""
                    SELECT COUNT(*)::integer AS "Value"
                    FROM "{tableName}"
                    """)
                .SingleAsync();

            count.Should().Be(1);
        }
        finally
        {
            await DropTestTableAsync(db, tableName);
        }
    }

    [Fact]
    public async Task RollbackAsync_RollsBackTransaction()
    {
        const string tableName = "UowRollbackTest";

        await using var db = CreateDbContext();

        await CreateTestTableAsync(db, tableName);

        try
        {
            var unitOfWork = new EfUnitOfWork(db);

            await unitOfWork.BeginTransactionAsync();

            await db.Database.ExecuteSqlRawAsync($"""
                INSERT INTO "{tableName}" ("Id")
                VALUES (1)
                """);

            await unitOfWork.RollbackAsync();

            await using var verificationDb = CreateDbContext();

            var count = await verificationDb.Database
                .SqlQueryRaw<int>($"""
                    SELECT COUNT(*)::integer AS "Value"
                    FROM "{tableName}"
                    """)
                .SingleAsync();

            count.Should().Be(0);
        }
        finally
        {
            await DropTestTableAsync(db, tableName);
        }
    }

    private RentFlowDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<RentFlowDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        return new RentFlowDbContext(options);
    }

    private static async Task CreateTestTableAsync(
        RentFlowDbContext db,
        string tableName)
    {
        await db.Database.ExecuteSqlRawAsync($"""
            CREATE TABLE "{tableName}" (
                "Id" integer NOT NULL
            )
            """);
    }

    private static async Task DropTestTableAsync(
        RentFlowDbContext db,
        string tableName)
    {
        await db.Database.ExecuteSqlRawAsync(
            $"""DROP TABLE IF EXISTS "{tableName}" """);
    }
}
using Npgsql;
using Microsoft.EntityFrameworkCore;
using RentFlow.Persistance;
using Respawn;
using Testcontainers.PostgreSql;

namespace RentFlow.Test.IntegrationTests.Infrastructure;

public class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private Respawner _respawner = null;

    public string ConnectionString => _container.GetConnectionString();

    public PostgreSqlContainerFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await ApplyMigrationsAsync();
        await InitializeRespawnerAsync();;
    }

    private async Task ApplyMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<RentFlowDbContext>().UseNpgsql(ConnectionString).Options;

        await using var db = new RentFlowDbContext(options);
        await db.Database.MigrateAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] {"public"}
        });
    }

    public async Task ResetDatabase()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

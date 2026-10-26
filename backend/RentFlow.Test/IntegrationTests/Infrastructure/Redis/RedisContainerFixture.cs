using StackExchange.Redis;
using Testcontainers.Redis;

namespace RentFlow.Test.IntegrationTests.Infrastructure.Redis;

public sealed class RedisContainerFixture : IAsyncLifetime
{
    private readonly RedisContainer _container =
        new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();

    public string ConnectionString => _container.GetConnectionString();
    public IConnectionMultiplexer Connection { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Connection = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
    }

    public async Task ResetDatabase()
    {
        var server = Connection.GetServer(
            Connection.GetEndPoints().Single());

        await server.FlushDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await Connection.DisposeAsync();
        await _container.DisposeAsync();
    }
}
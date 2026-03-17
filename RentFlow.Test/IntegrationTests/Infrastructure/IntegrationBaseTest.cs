using Microsoft.Extensions.DependencyInjection;
using RentFlow.Persistance;

namespace RentFlow.Test.IntegrationTests.Infrastructure;

[Collection("Postges collection")]
public abstract class IntegrationBaseTest : IClassFixture<PostgreSqlContainerFixture>
{
    protected readonly HttpClient Client;
    protected readonly RentFlowDbContext Db;
    protected readonly PostgreSqlContainerFixture Fixture;
    private readonly CustomWebApplicationFactory _factory;

    protected IntegrationBaseTest(PostgreSqlContainerFixture fixture)
    {
        Fixture = fixture;

        _factory = new CustomWebApplicationFactory(fixture.ConnectionString);
        Client = _factory.CreateClient();
    }

    protected async Task ResetDatabaseAsync() => await Fixture.ResetDatabase();

    protected async Task<RentFlowDbContext> CreateDbContextAsync()
    {

        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<RentFlowDbContext>();
    }
}

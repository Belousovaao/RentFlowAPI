using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using RentFlow.Persistance;
using Microsoft.Extensions.DependencyInjection;

namespace RentFlow.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services => 
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<RentFlowDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<RentFlowDbContext>(options => options.UseNpgsql(_connectionString));

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var db = serviceProvider.GetRequiredService<RentFlowDbContext>();
            db.Database.Migrate();
            
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

}

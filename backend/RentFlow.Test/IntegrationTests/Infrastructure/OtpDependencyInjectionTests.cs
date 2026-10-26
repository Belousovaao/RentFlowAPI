using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Moq;
using RentFlow.Application.Auth.Otp;
using RentFlow.Infrastructure.Redis;
using Microsoft.Extensions.Logging;

namespace RentFlow.Test.IntegrationTests.Infrastructure;

public sealed class OtpDependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_CanResolveOtpService()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Redis:ConnectionString"] = "localhost:6379",

                ["Otp:CodeLength"] = "6",
                ["Otp:MaxAttempts"] = "5",
                ["Otp:ChallengeTtlSeconds"] = "300",
                ["Otp:ResendCooldownSeconds"] = "60",

                ["OtpRateLimit:DestinationHourlyLimit"] = "5",
                ["OtpRateLimit:DestinationDailyLimit"] = "10",
                ["OtpRateLimit:IpTenMinuteLimit"] = "10",
                ["OtpRateLimit:IpHourlyLimit"] = "30",
                ["OtpRateLimit:IpDestinationHourlyLimit"] = "3",

                ["OtpSecurity:HmacKey"] =
                    Convert.ToBase64String(new byte[32])
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(
            configuration,
            new TestHostEnvironment());

        // Не подключаемся к реальному Redis.
        // Нам здесь важно проверить DI-граф, а не доступность Redis.
        services.AddSingleton(Mock.Of<StackExchange.Redis.IConnectionMultiplexer>());

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var otpService = scope.ServiceProvider.GetRequiredService<OtpService>();

        otpService.Should().NotBeNull();
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "RentFlow.Test";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; }  = new NullFileProvider();
    }
}
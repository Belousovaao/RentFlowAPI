using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Interfaces;
using RentFlow.Infrastructure.Redis;
using StackExchange.Redis;
using Microsoft.Extensions.Hosting;
using RentFlow.Application.Auth.Registration;

namespace RentFlow.Infrastructure.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        services.Configure<OtpOptions>(configuration.GetSection(OtpOptions.SectionName));
        services.Configure<OtpSecurityOptions>(configuration.GetSection(OtpSecurityOptions.SectionName));
        services.Configure<RegistrationOptions>(configuration.GetSection(RegistrationOptions.SectionName));
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp
                .GetRequiredService<IOptions<RedisOptions>>()
                .Value;

            return ConnectionMultiplexer.Connect(options.ConnectionString);
        });
        services.AddSingleton<IOtpStore, RedisOtpStore>();
        services.AddSingleton<IOtpRateLimiter, RedisOtpRateLimiter>();
        services.AddSingleton<IOtpSecurity, OtpSecurity>();
        services.AddSingleton<IOtpDestinationNormalizer, OtpDestinationNormalizer>();
        if (environment.IsDevelopment())
        {
            services.AddSingleton<IEmailOtpSender, DevelopmentEmailOtpSender>();
            services.AddSingleton<IPhoneOtpSender, DevelopmentPhoneOtpSender>();
        }
        services.AddSingleton<IOtpSender, OtpSender>();
        services.AddScoped<OtpService>(sp =>
        {
            var options = sp
                .GetRequiredService<IOptions<OtpOptions>>()
                .Value;

            return new OtpService(
                sp.GetRequiredService<IOtpSecurity>(),
                sp.GetRequiredService<IOtpRateLimiter>(),
                sp.GetRequiredService<IOtpStore>(),
                sp.GetRequiredService<IOtpSender>(),
                options,
                sp.GetRequiredService<IOtpDestinationNormalizer>());
        });
        services.AddScoped<IOtpService>(sp => sp.GetRequiredService<OtpService>());
        services.AddScoped<IOtpAuthenticationService, OtpAuthenticationService>();
        services.AddSingleton<IRegistrationTicketService, RedisRegistrationTicketService>();
        return services;
    }
}
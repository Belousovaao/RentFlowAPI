using RentFlow.Application.Interfaces;
using RentFlow.Persistance.Repositories;
using RentFlow.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RentFlow.Application.Bookings.Handlers;
using RentFlow.Persistance.Services;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Assets.Commands;
using RentFlow.Application.Bookings.Validators;
using FluentValidation;
using MediatR;
using RentFlow.Application.Common.Behaviors;
using RentFlow.Api.Middleware;
using Serilog;
using System.Text.Json;
using RentFlow.Infrastructure.Persistence.Repositories;
using System.Text;
using RentFlow.Application.Assets.Handlers;
using RentFlow.Application.Assets.Queries;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFramework", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithThreadName()
    .WriteTo.Async(a => a.Console())
    .WriteTo.Async(a => a.File(
        path: "logs/log-.json",
        rollingInterval: RollingInterval.Day,
        formatter: new Serilog.Formatting.Json.JsonFormatter()))
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddDbContext<RentFlowDbContext>((sp, opt) =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    opt
        .UseNpgsql(builder.Configuration.GetConnectionString("Db"))
        .UseLoggerFactory(loggerFactory)
        .EnableSensitiveDataLogging(false)
        .EnableDetailedErrors();

});

var assemblies = new[]
{
    typeof(CreateAssetCommand).Assembly,
    typeof(UploadAssetPhotoCommand).Assembly,
    typeof(CreateAssetCommandHandler).Assembly,
    typeof(CreateBookingCommand).Assembly,
    typeof(CreateBookingHandler).Assembly,
    typeof(GetAssetByCodeHandler).Assembly,
    typeof(GetAssetByIdQueryHandler).Assembly,
    typeof(UpdateAssetCommandHandler).Assembly,
    typeof(AddAssetPhotoCommandHandler).Assembly,
    typeof(Program).Assembly
}.Distinct();

builder.Services.AddMediatR(cfg => {
    foreach( var assembly in assemblies)
    {
        cfg.RegisterServicesFromAssembly(assembly);
    }
});
builder.Services.AddValidatorsFromAssembly(typeof(CreateBookingCommandValidator).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.Configure<GarageOptions>(builder.Configuration.GetSection(GarageOptions.SectionName));
builder.Services.AddScoped<IAssetPhotoStorage, GaragePhotoStorage>();
builder.Services.AddScoped<CreateBookingHandler>();
builder.Services.AddScoped<IAssetPhotoRepository, AssetPhotoRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonElement;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Настройка аутентификации JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "volnamidrive";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "volnamiDriveClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Читаем токен из cookie
            var token = context.Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            // Или из заголовка Authorization
            else
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader?.StartsWith("Bearer ") == true)
                {
                    context.Token = authHeader.Substring("Bearer ".Length);
                }
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}
builder.Services.AddProblemDetails();

string[]? allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RentFlowDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();
app.UseCors("AllowFrontend");

// заглушка для админских запросов для тестирования
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/api/admin"))
        {
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "admin"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "dev-admin")
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "Development");
            context.User = new System.Security.Claims.ClaimsPrincipal(identity);
        }
        await next();
    });
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

app.Run();

public partial class Program {}


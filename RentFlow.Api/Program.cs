using RentFlow.Application.Interfaces;
using RentFlow.Persistance.Repositories;
using RentFlow.Persistance;
using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Bookings.Handlers;
using RentFlow.Domain.Bookings.Services;
using RentFlow.Persistance.Services;
using RentFlow.Application.Bookings.Commands;
using RentFlow.Application.Bookings.Validators;
using FluentValidation;
using MediatR;
using RentFlow.Application.Common.Behaviors;
using RentFlow.Api.Middleware;
using Serilog;

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
    
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBookingCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateBookingCommandValidator).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<CreateBookingHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();

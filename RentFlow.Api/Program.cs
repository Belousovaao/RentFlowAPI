using RentFlow.Application.Interfaces;
using RentFlow.Persistance.Repositories;
using RentFlow.Persistance;

using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Bookings.Handlers;
using RentFlow.Domain.Bookings.Services;
using RentFlow.Persistance.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RentFlowDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Db")));

builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<CreateBookingHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler("/error");
app.UseStatusCodePagesWithReExecute("/error/{0}");

app.MapControllers();
app.Run();

using RentFlow.Application.Interfaces;
using RentFlow.Persistance.Repositories;
using RentFlow.Persistance;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RentFlowDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Db")));

builder.Services.AddScoped<IAssetRepository, AssetRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();

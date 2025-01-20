using FinCtrlApi.Databases.MongoDb.FinCtrl;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using FinCtrlApi.Entities;
using FinCtrlLibrary.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(EntitiesHelperClass.DebugBuild ? "log.txt" : "C:\\log.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

string? mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDbString");

if (mongoDbConnectionString == null)
{
    Console.WriteLine("String de conexão ao banco não setada.");
    Environment.Exit(0);
}

// Add services to the container.
builder.Services.AddDbContextPool<FinCtrlAppDbContext>(
    options => options.UseMongoDB(mongoDbConnectionString, "FinCtrl"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ICategory, CategoryRepository>();

var app = builder.Build();

app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

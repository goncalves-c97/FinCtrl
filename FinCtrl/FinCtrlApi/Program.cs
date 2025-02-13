using FinCtrlApi.Databases.MongoDb.FinCtrl;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using FinCtrlApi.Entities;
using FinCtrlLibrary.Middlewares;
using FinCtrlLibrary.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Conventions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(EntitiesHelperClass.DebugBuild ? "log.txt" : "C:\\log.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

ConfigureMongoDB();

string? mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDbString");

if (mongoDbConnectionString == null)
{
    Console.WriteLine("String de conexão ao banco não setada.");
    Environment.Exit(0);
}

// Add services to the container.
builder.Services.AddDbContextPool<FinCtrlAppDbContext>(
    options => options.UseMongoDB(mongoDbConnectionString, "finctrl_dtb"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenericRepository<SpendingCategory>, GenericRepository<SpendingCategory>>();
builder.Services.AddScoped<IGenericRepository<PaymentCategory>, GenericRepository<PaymentCategory>>();
builder.Services.AddScoped<IGenericRepository<TagCategory>, GenericRepository<TagCategory>>();
builder.Services.AddScoped<IGenericRepository<SpendingRule>, GenericRepository<SpendingRule>>();
builder.Services.AddScoped<IGenericRepository<EarningCategory>, GenericRepository<EarningCategory>>();
builder.Services.AddScoped<ISpendingRecordRepository, SpendingRecordRepository>();
builder.Services.AddScoped<IEarningRecordRepository, EarningRecordRepository>();


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

// MongoDB configuration method
static void ConfigureMongoDB()
{
    var conventionPack = new ConventionPack
    {
        new IgnoreIfNullConvention(true),
        new IgnoreExtraElementsConvention(true)
    };
    ConventionRegistry.Register(
        "IgnoreDefaultValues",
        conventionPack,
        type => true // Apply to all types
    );
}

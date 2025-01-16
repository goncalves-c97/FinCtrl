

using FinCtrlApi.Databases.MongoDb.FinCtrl;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

string? mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDbString");

if (mongoDbConnectionString == null)
{
    Console.WriteLine("String de conexão ao banco não setada.");
    Environment.Exit(0);
}

//MongoClient client = new(mongoDbConnectionString);
//FinCtrlAppDbContext db = FinCtrlAppDbContext.Create(client.GetDatabase("FinCtrl"));

// Add services to the container.
builder.Services.AddDbContextPool<FinCtrlAppDbContext>(
    options => options.UseMongoDB(mongoDbConnectionString, "FinCtrl"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ICategory, CategoryRepository>();

var app = builder.Build();

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

using FinCtrlApi.Databases.MongoDb.FinCtrl;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using FinCtrlApi.Entities;
using FinCtrlApi.Services;
using FinCtrlLibrary.Exceptions;
using FinCtrlLibrary.Middlewares;
using FinCtrlLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(EntitiesHelperClass.DebugBuild ? "log.txt" : "C:\\log.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

#region Database Configuration

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

#endregion

#region TokenService Configuration

string? tokenSecret = builder.Configuration.GetValue<string>("TokenServiceSecret");

if (string.IsNullOrEmpty(tokenSecret))
    throw new AppSettingsValueNotFoundException("'TokenServiceSecret' não encontrado");

TokenService.SetSecret(tokenSecret);

int? tokenHoursToExpire = builder.Configuration.GetValue<int?>("TokenServiceHoursToExpire");

if(tokenHoursToExpire == null)
    throw new AppSettingsValueNotFoundException("'TokenServiceHoursToExpire' não encontrado");

TokenService.SetHoursToExpire((int)tokenHoursToExpire);

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Interfaces/Repositories Configuration

builder.Services.AddScoped<IGenericRepository<SpendingCategory>, GenericRepository<SpendingCategory>>();
builder.Services.AddScoped<IGenericRepository<PaymentCategory>, GenericRepository<PaymentCategory>>();
builder.Services.AddScoped<IGenericRepository<TagCategory>, GenericRepository<TagCategory>>();
builder.Services.AddScoped<IGenericRepository<SpendingRule>, GenericRepository<SpendingRule>>();
builder.Services.AddScoped<IGenericRepository<EarningCategory>, GenericRepository<EarningCategory>>();
builder.Services.AddScoped<ISpendingRecordRepository, SpendingRecordRepository>();
builder.Services.AddScoped<IEarningRecordRepository, EarningRecordRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

#endregion

byte[] key = Encoding.ASCII.GetBytes(tokenSecret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

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

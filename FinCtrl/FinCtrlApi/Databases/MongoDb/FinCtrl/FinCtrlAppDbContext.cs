using FinCtrlLibrary.Models;
using FinCtrlLibrary.Validators;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl
{
    public class FinCtrlAppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; init; }

        public static FinCtrlAppDbContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<FinCtrlAppDbContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);

        public FinCtrlAppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToCollection("categories");
        }
    }

}


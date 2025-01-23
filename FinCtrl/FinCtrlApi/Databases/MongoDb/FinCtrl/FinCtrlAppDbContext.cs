using FinCtrlLibrary.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl
{
    public class FinCtrlAppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; init; }
        public DbSet<SpendingRecord> SpendingRecords { get; init; }

        public static FinCtrlAppDbContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<FinCtrlAppDbContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);

        public FinCtrlAppDbContext(DbContextOptions options) : base(options)
        {
            var conventionPack = new ConventionPack
    {
        new IgnoreIfDefaultConvention(true)
    };

            ConventionRegistry.Register(
                "IgnoreDefaultValues",
                conventionPack,
                type => true // Apply to all types
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToCollection("categories");
            modelBuilder.Entity<SpendingRecord>().ToCollection("spendingRecords");

            //modelBuilder.Entity<SpendingRecord>().HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryBsonId);
        }
    }

}


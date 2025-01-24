using FinCtrlLibrary.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinCtrlApi.Databases.MongoDb.FinCtrl
{
    public class FinCtrlAppDbContext : DbContext
    {
        public DbSet<SpendingCategory> SpendingCategories { get; init; }
        public DbSet<PaymentCategory> PaymentCategories { get; init; }
        public DbSet<TagCategory> TagCategories { get; init; }
        public DbSet<SpendingRule> SpendingRule { get; init; }
        public DbSet<SpendingRecord> SpendingRecords { get; init; }

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

            modelBuilder.Entity<SpendingCategory>().ToCollection("spendingCategories");
            modelBuilder.Entity<PaymentCategory>().ToCollection("paymentCategories");
            modelBuilder.Entity<TagCategory>().ToCollection("tagCategories");
            modelBuilder.Entity<SpendingRule>().ToCollection("spendingRule");
            modelBuilder.Entity<SpendingRecord>().ToCollection("spendingRecords");
        }
    }

}


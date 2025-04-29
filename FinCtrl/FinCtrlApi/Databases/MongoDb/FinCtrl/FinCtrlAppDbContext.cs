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
        public DbSet<EarningCategory> EarningCategories { get; init; }
        public DbSet<EarningRecord> EarningRecords { get; init; }
        public DbSet<User> Users { get; init; }
        public DbSet<Authentication> Authentications { get; init; }

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

            modelBuilder.Entity<SpendingCategory>().ToCollection("spending_categories");
            modelBuilder.Entity<PaymentCategory>().ToCollection("payment_categories");
            modelBuilder.Entity<TagCategory>().ToCollection("tag_categories");
            modelBuilder.Entity<SpendingRule>().ToCollection("spending_rule");
            modelBuilder.Entity<SpendingRecord>().ToCollection("spending_records");
            modelBuilder.Entity<EarningCategory>().ToCollection("earning_categories");
            modelBuilder.Entity<EarningRecord>().ToCollection("earning_records");
            modelBuilder.Entity<User>().ToCollection("user");
            modelBuilder.Entity<Authentication>().ToCollection("authentication");
        }
    }

}


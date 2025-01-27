using FinCtrlApi.Databases.MongoDb.FinCtrl;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using FinCtrlLibrary.Models;
using MongoDB.Bson;
using Xunit.Abstractions;

namespace FinCtrl.Test.Integracao
{
    [Collection(nameof(ContextCollection))]
    public class GenericRepositoryTests
    {
        private readonly FinCtrlAppDbContext _context;
        private readonly GenericRepository<SpendingCategory> _spendingCategoryRepo;
        private readonly GenericRepository<PaymentCategory> _paymentCategoryRepo;
        private readonly GenericRepository<TagCategory> _tagCategoryRepo;
        private readonly GenericRepository<SpendingRule> _spendingRuleRepo;

        public GenericRepositoryTests(ITestOutputHelper output, ContextFixture contextFixture) 
        {
            _context = contextFixture.Context;

            _spendingCategoryRepo = new GenericRepository<SpendingCategory>(_context);
            _paymentCategoryRepo = new GenericRepository<PaymentCategory>(_context);
            _tagCategoryRepo = new GenericRepository<TagCategory>(_context);
            _spendingRuleRepo = new GenericRepository<SpendingRule>(_context);

            output.WriteLine(_context.GetHashCode().ToString());
        }

        [Fact]
        public async Task ReturnsSuccessWhenInsertingAndDeletingSpendingCategoryAsync()
        {
            int categoryId = 1;
            string categoryName = "Bar";
            GenericRepository<SpendingCategory> genericRepository = _spendingCategoryRepo;

            SpendingCategory category = new(categoryId, categoryName);

            await genericRepository.InsertNewAsync(category);

            _context.ChangeTracker.Clear();

            Assert.NotEqual(ObjectId.Empty, category._id);

            await genericRepository.DeleteByIdAsync(category._id.ToString());

            _context.ChangeTracker.Clear();

            SpendingCategory? deletedCategory = await genericRepository.GetByIdAsync(category._id.ToString());

            Assert.Null(deletedCategory);
        }

        [Fact]
        public async Task ReturnsSuccessWhenInsertingAndDeletingPaymentCategoryAsync()
        {
            int categoryId = 1;
            string categoryName = "Pix";
            GenericRepository<PaymentCategory> genericRepository = _paymentCategoryRepo;

            PaymentCategory category = new(categoryId, categoryName);

            await genericRepository.InsertNewAsync(category);

            _context.ChangeTracker.Clear();

            Assert.NotEqual(ObjectId.Empty, category._id);

            await genericRepository.DeleteByIdAsync(category._id.ToString());

            _context.ChangeTracker.Clear();

            PaymentCategory? deletedCategory = await genericRepository.GetByIdAsync(category._id.ToString());

            Assert.Null(deletedCategory);
        }

        [Fact]
        public async Task ReturnsSuccessWhenInsertingAndDeletingTagCategoryAsync()
        {
            int categoryId = 1;
            string categoryName = "Viagem Disney 2015";
            GenericRepository<TagCategory> genericRepository = _tagCategoryRepo;

            TagCategory category = new(categoryId, categoryName);

            await genericRepository.InsertNewAsync(category);

            _context.ChangeTracker.Clear();

            Assert.NotEqual(ObjectId.Empty, category._id);

            await genericRepository.DeleteByIdAsync(category._id.ToString());

            _context.ChangeTracker.Clear();

            TagCategory? deletedCategory = await genericRepository.GetByIdAsync(category._id.ToString());

            Assert.Null(deletedCategory);
        }
    }
}
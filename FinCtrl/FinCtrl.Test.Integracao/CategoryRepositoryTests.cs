using FinCtrlApi.Databases.MongoDb.FinCtrl;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using FinCtrlLibrary.Models;
using MongoDB.Bson;
using Xunit.Abstractions;

namespace FinCtrl.Test.Integracao
{
    [Collection(nameof(ContextCollection))]
    public class CategoryRepositoryTests
    {
        private readonly FinCtrlAppDbContext _context;
        private readonly GenericRepository<SpendingCategory> _repo;

        public CategoryRepositoryTests(ITestOutputHelper output, ContextFixture contextFixture) 
        {
            _context = contextFixture.Context;
            _repo = new GenericRepository<SpendingCategory>(_context);
            output.WriteLine(_context.GetHashCode().ToString());
        }

        [Fact]
        public async Task InsertNewCategoryAsync()
        {
            int categoryId = 2;
            string categoryName = "Bar";

            SpendingCategory category = new(categoryId, categoryName);

            await _repo.InsertNewAsync(category);

            _context.ChangeTracker.Clear();

            Assert.NotEqual(ObjectId.Empty, category._id);
        }

        [Fact]
        public async Task DeleteCategoryAsync()
        {
            int categoryId = 2;
            string categoryName = "Bar";

            SpendingCategory category = new(categoryId, categoryName);

            //await _repo.DeleteByIdAsync(category);

            _context.ChangeTracker.Clear();

            //Category? deletedCategory = await _repo.GetByIdAsync(category.Id);

            //Assert.Null(deletedCategory);
        }
    }
}
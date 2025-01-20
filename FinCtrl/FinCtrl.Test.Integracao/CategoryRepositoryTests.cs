using FinCtrlApi.Databases.MongoDb.FinCtrl;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;
using FinCtrlLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Xunit.Abstractions;

namespace FinCtrl.Test.Integracao
{
    [Collection(nameof(ContextCollection))]
    public class CategoryRepositoryTests
    {
        private readonly FinCtrlAppDbContext _context;
        private readonly CategoryRepository _repo;

        public CategoryRepositoryTests(ITestOutputHelper output, ContextFixture contextFixture) 
        {
            _context = contextFixture.Context;
            _repo = new CategoryRepository(_context);
            output.WriteLine(_context.GetHashCode().ToString());
        }

        [Fact]
        public void InsertNewCategory()
        {
            int categoryId = 2;
            string categoryName = "Bar";

            Category category = new(categoryId, categoryName);

            _repo.InsertNew(category);

            _context.ChangeTracker.Clear();

            Assert.NotEqual(ObjectId.Empty, category._id);

            //_repo.DeleteById(category.Id);

            //_context.ChangeTracker.Clear();

            //Category? deletedCategory = _repo.GetById(category.Id);

            //Assert.Null(deletedCategory);
        }

        [Fact]
        public void DeleteCategory()
        {
            int categoryId = 2;
            string categoryName = "Bar";

            Category category = new(categoryId, categoryName);

            _repo.DeleteById(category.Id);

            _context.ChangeTracker.Clear();

            Category? deletedCategory = _repo.GetById(category.Id);

            Assert.Null(deletedCategory);
        }
    }
}
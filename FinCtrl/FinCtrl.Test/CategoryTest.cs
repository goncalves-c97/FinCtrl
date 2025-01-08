using FinCtrlLibrary.Models;

namespace FinCtrl.Test
{
    public class CategoryTest
    {
        [Fact]
        public void TestingValidCategory()
        {
            int categoryId = 1;
            string categoryName = "Mercado";
            bool validation = true;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
        }

        [Fact]
        public void TestingNegativeIdCategory()
        {
            int categoryId = -10;
            string categoryName = "Mercado";
            bool validation = false;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
        }

        [Fact]
        public void TestingEmptyDescriptionCategory()
        {
            int categoryId = 0;
            string categoryName = string.Empty;
            bool validation = false;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
        }

        [Fact]
        public void TestingTooLargeDescriptionCategory()
        {
            int categoryId = 1;
            string categoryName = "Esse é um nome de categoria que supera o tamanho limite estipulado para uma categoria.";
            bool validation = false;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
        }
    }
}
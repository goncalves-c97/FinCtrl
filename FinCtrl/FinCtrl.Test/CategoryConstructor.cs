using FinCtrlLibrary.Models;
using FinCtrlLibrary.Validators;

namespace FinCtrl.Test
{
    public class CategoryConstructor
    {
        [Fact]
        public void ReturnsValidCategoryWhenDataIsValid()
        {
            int categoryId = 1;
            string categoryName = "Mercado";
            bool validation = true;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
        }

        [Fact]
        public void ReturnsNegativeCategoryIdErrorWhenCategoryIdIsNull()
        {
            int categoryId = -10;
            string categoryName = "Mercado";
            bool validation = false;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
            Assert.True(categoryTest.ContainsError(GenericErrors.NegativeIdError, nameof(categoryTest.Id)));
        }

        [Fact]
        public void ReturnsEmptyCategoryNameErrorWhenCategoryNameIsEmpty()
        {
            int categoryId = 0;
            string categoryName = string.Empty;
            bool validation = false;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
            Assert.True(categoryTest.ContainsError(GenericErrors.EmptyStringError, nameof(categoryTest.Name)));
        }

        [Fact]
        public void ReturnsCategoryNameMaxLengthExceededErrorWhenCategoryNameExceedsMaxLength()
        {
            int categoryId = 1;
            string categoryName = "Esse é um nome de categoria que supera o tamanho limite estipulado para uma categoria.";
            bool validation = false;

            Category categoryTest = new(categoryId, categoryName);

            Assert.Equal(validation, categoryTest.IsValid);
            Assert.True(categoryTest.ContainsError(GenericErrors.StringSizeExcedeedError, nameof(categoryTest.Name)));
        }
    }
}
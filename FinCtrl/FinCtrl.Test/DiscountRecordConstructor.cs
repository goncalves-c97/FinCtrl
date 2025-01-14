using FinCtrlLibrary.Models;
using FinCtrlLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCtrl.Test
{
    public class DiscountRecordConstructor
    {
        [Fact]
        public void ReturnsValidDiscountRecordWhenDataIsValid()
        {
            long discountRecordId = 1;
            int spendingRecordId = 2;
            int? discountRecordCategoryId = 3;
            double discountValue = 49.99;
            bool validation = true;

            DiscountRecord discountRecord = new(discountRecordId, spendingRecordId, discountRecordCategoryId, discountValue);

            Assert.Equal(validation, discountRecord.IsValid);
        }

        [Fact]
        public void ReturnsValidDiscountRecordWhenDicountountRecordCategoryIdIsNull()
        {
            long discountRecordId = 1;
            int spendingRecordId = 2;
            int? discountRecordCategoryId = null;
            double discountValue = 49.99;
            bool validation = true;

            DiscountRecord discountRecord = new(discountRecordId, spendingRecordId, discountRecordCategoryId, discountValue);

            Assert.Equal(validation, discountRecord.IsValid);
        }

        [Fact]
        public void ReturnsNegativeValueErrorWhenDiscountRecordDiscountIsNegative()
        {
            long discountRecordId = 1;
            int spendingRecordId = 2;
            int? discountRecordCategoryId = 3;
            double discountValue = -49.99;
            bool validation = false;

            DiscountRecord discountRecord = new(discountRecordId, spendingRecordId, discountRecordCategoryId, discountValue);

            Assert.Equal(validation, discountRecord.IsValid);
            Assert.True(discountRecord.ContainsError(GenericErrors.NegativeValueError, nameof(discountRecord.DiscountValue)));
        }
    }
}

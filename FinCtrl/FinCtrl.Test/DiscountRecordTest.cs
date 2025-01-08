using FinCtrlLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCtrl.Test
{
    public class DiscountRecordTest
    {
        [Fact]
        public void TestingValidDiscountRecord()
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
        public void TestingValidDiscountRecordWithNullDiscountRecordCategoryId()
        {
            long discountRecordId = 1;
            int spendingRecordId = 2;
            int? discountRecordCategoryId = null;
            double discountValue = 49.99;
            bool validation = true;

            DiscountRecord discountRecord = new(discountRecordId, spendingRecordId, discountRecordCategoryId, discountValue);

            Assert.Equal(validation, discountRecord.IsValid);
        }
    }
}

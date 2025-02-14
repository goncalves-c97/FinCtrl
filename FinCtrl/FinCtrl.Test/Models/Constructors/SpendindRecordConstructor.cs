﻿using FinCtrlLibrary.Models;
using FinCtrlLibrary.Validators;

namespace FinCtrl.Test.Models.Constructors
{
    public class SpendindRecordConstructor
    {
        [Fact]
        public void ReturnsValidSpendingRecordWhenDataIsValid()
        {
            int id = 1;
            DateTime dateTime = DateTime.Now;
            int paymentCategoryId = 2;
            int installments = 3;
            int? categoryId = 4;
            List<int>? tagIds = [1, 2, 3, 4];
            string description = "Gasolina";
            List<long>? discountRecordsIds = [5, 6, 7, 8];
            double unitValue = 0;
            double originalValue = 249.99;
            int? ruleId = 5;
            bool paid = false;
            bool validation = true;

            SpendingRecord spendingRecord = new(id, dateTime, paymentCategoryId, installments, categoryId, tagIds, description, discountRecordsIds, unitValue, originalValue, ruleId, paid);

            Assert.Equal(validation, spendingRecord.IsValid);
        }

        [Fact]
        public void ReturnsValidSpendingRecordWhenValidDataWithExpectedNullValues()
        {
            int id = 1;
            DateTime dateTime = DateTime.Now;
            int paymentCategoryId = 2;
            int installments = 3;
            int? categoryId = null;
            List<int>? tagIds = null;
            string description = "Gasolina";
            List<long>? discountRecordsIds = null;
            double unitValue = 0;
            double originalValue = 249.99;
            int? ruleId = null;
            bool paid = false;
            bool validation = true;

            SpendingRecord spendingRecord = new(id, dateTime, paymentCategoryId, installments, categoryId,
                tagIds, description, discountRecordsIds, unitValue, originalValue, ruleId, paid);

            Assert.Equal(validation, spendingRecord.IsValid);
        }

        [Fact]
        public void ReturnsNegativeIdsErrorsWhenPossibleIdsAreNegative()
        {
            int id = -1;
            DateTime dateTime = DateTime.Now;
            int paymentCategoryId = -2;
            int installments = 3;
            int? categoryId = -4;
            List<int>? tagIds = [-1, 2, 3, 4];
            string description = "Gasolina";
            List<long>? discountRecordsIds = [5, 6, 7, -8];
            double unitValue = 0;
            double originalValue = 249.99;
            int? ruleId = -5;
            bool paid = false;
            bool validation = false;

            SpendingRecord spendingRecord = new(id, dateTime, paymentCategoryId, installments, categoryId,
                tagIds, description, discountRecordsIds, unitValue, originalValue, ruleId, paid);

            Assert.Equal(validation, spendingRecord.IsValid);
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeIdError, nameof(spendingRecord.Id)));
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeIdError, nameof(spendingRecord.PaymentCategoryId)));
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeIdError, nameof(spendingRecord.CategoryId)));
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeIdError, nameof(spendingRecord.TagIds)));
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeIdError, nameof(spendingRecord.DiscountRecordsIds)));
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeIdError, nameof(spendingRecord.SpendingRuleId)));
        }

        [Fact]
        public void ReturnsNegativeValuesErrorsWhenPossibleValuesAreNegative()
        {
            int id = 1;
            DateTime dateTime = DateTime.Now;
            int paymentCategoryId = 2;
            int installments = -3;
            int? categoryId = 4;
            List<int>? tagIds = [1, 2, 3, 4];
            string description = "Gasolina";
            List<long>? discountRecordsIds = [5, 6, 7, 8];
            double unitValue = 0;
            double originalValue = -249.99;
            int? ruleId = 5;
            bool paid = false;
            bool validation = false;

            SpendingRecord spendingRecord = new(id, dateTime, paymentCategoryId, installments, categoryId,
                tagIds, description, discountRecordsIds, unitValue, originalValue, ruleId, paid);

            Assert.Equal(validation, spendingRecord.IsValid);
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeValueError, nameof(spendingRecord.Installments)));
            Assert.True(spendingRecord.ContainsError(GenericErrors.NegativeValueError, nameof(spendingRecord.OriginalValue)));
        }
    }
}

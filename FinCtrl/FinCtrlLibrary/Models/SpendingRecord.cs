﻿using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace FinCtrlLibrary.Models
{
    public class SpendingRecord : ValidatorClass, IMongoDbItem
    {
        private const int _descriptionMaxLength = 100;

        [BsonId]
        public ObjectId _id { get; set; }
        [JsonIgnore, BsonIgnore]
        public long Id { get; set; }
        public DateTime DateTime { get; set; }
        public string PaymentCategoryBsonId { get; set; }
        [BsonIgnore]
        public int PaymentCategoryId { get; set; }
        public int Installments { get; set; }
        [JsonIgnore, BsonIgnore]
        public int? CategoryId { get; set; }
        public string? CategoryBsonId { get; set; }
        [JsonIgnore, BsonIgnore]
        public List<int>? TagIds { get; set; } = [];
        public List<string>? TagBsonIds { get; set; } = [];
        public string Description { get; set; }
        [JsonIgnore, BsonIgnore]
        public List<long>? DiscountRecordsIds { get; set; } = [];
        public List<long>? DiscountRecordsBsonIds { get; set; } = [];
        public double UnitValue { get; set; }
        public double OriginalValue { get; set; }
        [JsonIgnore, BsonIgnore]
        public int? SpendingRuleId { get; set; }
        public string? SpendingRuleBsonId { get; set; }
        public bool Paid { get; set; }

        [BsonIgnore]
        public PaymentCategory PaymentCategory { get; set; }
        [BsonIgnore]
        public SpendingCategory? SpendingCategory { get; set; }
        [BsonIgnore]
        public List<TagCategory>? TagCategories { get; set; } = [];
        [BsonIgnore]
        public List<DiscountRecord>? DiscountRecords { get; set; } = [];
        [BsonIgnore]
        public SpendingRule? SpendingRule { get; set; }

        public SpendingRecord()
        {

        }

        public SpendingRecord(int id, DateTime dateTime, int paymentCategoryId, int installments, int? categoryId,
            List<int>? tagIds, string description, List<long>? discountRecordsIds, double unitValue, double originalValue, int? ruleId, bool paid)
        {
            Id = id;
            DateTime = dateTime;
            PaymentCategoryId = paymentCategoryId;
            Installments = installments;
            CategoryId = categoryId;
            TagIds = tagIds;
            Description = description;
            DiscountRecordsIds = discountRecordsIds;
            UnitValue = unitValue;
            OriginalValue = originalValue;
            SpendingRuleId = ruleId;
            Paid = paid;
            Validate();
        }

        public SpendingRecord(int id, DateTime dateTime, PaymentCategory paymentCategory, int installments, SpendingCategory? category,
            TagCategory? tag, string description, List<DiscountRecord>? discountRecords, double unitValue, double originalValue, SpendingRule? rule, bool paid)
        {
            Id = id;
            DateTime = dateTime;

            if (paymentCategory != null)
            {
                PaymentCategoryId = paymentCategory.Id;
                PaymentCategory = paymentCategory;
            }

            Installments = installments;

            if (category != null)
            {
                CategoryId = category.Id;
                SpendingCategory = category;
            }

            if (tag != null)
            {
                TagIds.Add(tag.Id);
                TagCategories.Add(tag);
            }

            Description = description;

            if (discountRecords != null && discountRecords.Count > 0)
            {
                DiscountRecordsIds = discountRecords.Select(x => x.Id).ToList();
                DiscountRecords = discountRecords;
            }

            UnitValue = unitValue;
            OriginalValue = originalValue;

            if (rule != null)
            {
                SpendingRuleId = rule.Id;
                SpendingRule = rule;
            }

            Paid = paid;
            Validate();
        }

        protected override void Validate()
        {
            IdValidation(Id, nameof(Id));
            IdValidation(PaymentCategoryId, nameof(PaymentCategoryId), true);
            PositiveValueValidation(nameof(Installments), Installments);

            if (CategoryId != null)
                IdValidation((int)CategoryId, nameof(CategoryId), true);

            if (TagIds != null)
            {
                foreach (int tagId in TagIds)
                {
                    if (tagId == 0)
                    {
                        Errors.RegisterError(GenericErrors.IdZeroError, $"'{nameof(TagIds)}' possui um ou mais elementos como 0.", nameof(TagIds));
                        break;
                    }
                    else if (int.IsNegative(tagId))
                    {
                        Errors.RegisterError(GenericErrors.NegativeIdError, $"'{nameof(TagIds)}' possui um ou mais elementos negativos.'", nameof(TagIds));
                        break;
                    }
                }
            }

            NotEmptyStringLengthValidation(nameof(Description), Description, _descriptionMaxLength);

            if (DiscountRecordsIds != null)
            {
                foreach (int discountRecordId in DiscountRecordsIds)
                {
                    if (discountRecordId == 0)
                    {
                        Errors.RegisterError(GenericErrors.IdZeroError, $"'{nameof(DiscountRecordsIds)}' possui um ou mais elementos como 0.", nameof(DiscountRecordsIds));
                        break;
                    }
                    else if (int.IsNegative(discountRecordId))
                    {
                        Errors.RegisterError(GenericErrors.NegativeIdError, $"'{nameof(DiscountRecordsIds)}' possui um ou mais elementos negativos.'", nameof(DiscountRecordsIds));
                        break;
                    }
                }
            }

            PositiveValueValidation(nameof(OriginalValue), OriginalValue);

            if (SpendingRuleId != null)
                IdValidation((int)SpendingRuleId, nameof(SpendingRuleId), true);
        }

        public override bool Equals(object? obj)
        {
            bool isSpendingRecord = obj is SpendingRecord;

            SpendingRecord spendingRecord = obj as SpendingRecord;

            return isSpendingRecord && DateTime.ToShortDateString() == spendingRecord.DateTime.ToShortDateString() && Description.Equals(spendingRecord.Description, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return $"{DateTime.ToShortDateString()} - {(SpendingCategory != null ? SpendingCategory.Name : CategoryBsonId)} - {Description}";
        }
    }
}

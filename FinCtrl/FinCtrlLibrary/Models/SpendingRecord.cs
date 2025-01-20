using FinCtrlLibrary.Validators;
using System.Data;

namespace FinCtrlLibrary.Models
{
    public class SpendingRecord : Validator
    {
        private const int _descriptionMaxLength = 100;

        public long Id { get; set; }
        public DateTime DateTime { get; set; }
        public int SpendingPaymentCategoryId { get; set; }
        public int Installments { get; set; }
        public int? CategoryId { get; set; }
        public List<int>? TagIds { get; set; } = [];
        public string Description { get; set; }
        public List<long>? DiscountRecordsIds { get; set; } = [];
        public double UnitValue { get; set; }
        public double OriginalValue { get; set; }
        public int? SpendingRuleId { get; set; }
        public bool Paid { get; set; }

        public Category SpendingPaymentCategory { get; set; }
        public Category? Category { get; set; }
        public List<Category>? Tags { get; set; } = [];
        public List<DiscountRecord>? DiscountRecords { get; set; } = [];
        public SpendingRule? SpendingRule { get; set; }

        public SpendingRecord(int id, DateTime dateTime, int spendingPaymentCategoryId, int installments, int? categoryId,
            List<int>? tagIds, string description, List<long>? discountRecordsIds, double unitValue, double originalValue, int? ruleId, bool paid)
        {
            Id = id;
            DateTime = dateTime;
            SpendingPaymentCategoryId = spendingPaymentCategoryId;
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

        public SpendingRecord(int id, DateTime dateTime, Category spendingPaymentCategory, int installments, Category? category,
            Category? tag, string description, List<DiscountRecord>? discountRecords, double unitValue, double originalValue, SpendingRule? rule, bool paid)
        {
            Id = id;
            DateTime = dateTime;

            if (spendingPaymentCategory != null)
            {
                SpendingPaymentCategoryId = spendingPaymentCategory.Id;
                SpendingPaymentCategory = spendingPaymentCategory;
            }

            Installments = installments;

            if (category != null)
            {
                CategoryId = category.Id;
                Category = category;
            }

            if (tag != null)
            {
                TagIds.Add(tag.Id);
                Tags.Add(tag);
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
            IdValidation(SpendingPaymentCategoryId, nameof(SpendingPaymentCategoryId), true);
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
    }
}

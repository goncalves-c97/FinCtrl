using FinCtrlLibrary.Validators;

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
        public List<int>? TagIds { get; set; }
        public string Description { get; set; }
        public List<int>? DiscountRecordsIds { get; set; }
        public double OriginalValue { get; set; }
        public int? RuleId { get; set; }
        public bool Paid { get; set; }

        public Category SpendingPaymentCategory { get; set; }
        public Category? Category { get; set; }
        public List<Category>? Tags { get; set; }
        public List<DiscountRecord>? DiscountRecords { get; set; }

        public SpendingRecord(int id, DateTime dateTime, int spendingPaymentCategoryId, int installments, int? categoryId, 
            List<int>? tagIds, string description, List<int>? discountRecordsIds, double originalValue, int? ruleId, bool paid)
        {
            Id = id;
            DateTime = dateTime;
            SpendingPaymentCategoryId = spendingPaymentCategoryId;
            Installments = installments;
            CategoryId = categoryId;
            TagIds = tagIds;
            Description = description;
            DiscountRecordsIds = discountRecordsIds;
            OriginalValue = originalValue;
            RuleId = ruleId;
            Paid = paid;
            Validate();
        }

        protected override void Validate()
        {
            IdValidation(Id, nameof(Id));
            IdValidation(SpendingPaymentCategoryId, nameof(SpendingPaymentCategoryId), true);
            PositiveValueValidation(nameof(Installments), Installments);

            if(CategoryId != null)
                IdValidation((int)CategoryId, nameof(CategoryId), true);

            if(TagIds != null)
            { 
                foreach(int tagId in TagIds)
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

            if (RuleId != null)
                IdValidation((int)RuleId, nameof(RuleId), true);
        }
    }
}

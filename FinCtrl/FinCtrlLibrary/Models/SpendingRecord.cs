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
        public required string Description { get; set; }
        public List<int>? DiscountRecordsIds { get; set; }
        public double OriginalValue { get; set; }
        public int? RuleId { get; set; }
        public bool Paid { get; set; }

        public Category SpendingPaymentCategory { get; set; }
        public Category? Category { get; set; }
        public List<Category>? Tags { get; set; }
        public List<DiscountRecord>? DiscountRecords { get; set; }

        protected override void Validate()
        {
            IdValidation(Id);
            PositiveValueValidation(nameof(SpendingPaymentCategoryId), SpendingPaymentCategoryId);
            PositiveValueValidation(nameof(Installments), Installments);

            if(CategoryId != null)
                PositiveValueValidation(nameof(CategoryId), (int)CategoryId);

            if(TagIds != null)
            { 
                foreach(int tagId in TagIds)
                {
                    if (tagId == 0)
                    {
                        Errors.RegisterError(GenericErrors.ValueZeroError, $"'{nameof(TagIds)}' possui um elemento como 0.", nameof(TagIds));
                        break;
                    }
                    else if (int.IsNegative(tagId))
                    {
                        Errors.RegisterError(GenericErrors.NegativeValueError, $"'{nameof(TagIds)}' possui um elemento negativo.'", nameof(TagIds));
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
                        Errors.RegisterError(GenericErrors.ValueZeroError, $"'{nameof(DiscountRecordsIds)}' possui um elemento como 0.", nameof(DiscountRecordsIds));
                        break;
                    }
                    else if (int.IsNegative(discountRecordId))
                    {
                        Errors.RegisterError(GenericErrors.NegativeValueError, $"'{nameof(DiscountRecordsIds)}' possui um elemento negativo.'", nameof(DiscountRecordsIds));
                        break;
                    }
                }
            }

            PositiveValueValidation(nameof(OriginalValue), OriginalValue);

            if (RuleId != null)
                PositiveValueValidation(nameof(RuleId), (int)RuleId);
        }
    }
}

using FinCtrlLibrary.Validators;

namespace FinCtrlLibrary.Models
{
    public class DiscountRecord : ValidatorClass
    {
        public long Id { get; set; }
        public int SpendingRecordId { get; set; }
        public int? DiscountCategoryId { get; set; }
        public double DiscountValue { get; set; }

        public SpendingRecord SpendingRecord { get; set; }
        public Category? DiscountCategory { get; set; }

        public DiscountRecord(long id, int spendingRecordId, double discountValue) 
        {
            Id = id;
            SpendingRecordId = spendingRecordId;
            DiscountValue = discountValue;
            Validate();
        }

        public DiscountRecord(long id, int spendingRecordId, int? discountCategoryId, double discountValue)
        {
            Id = id;
            SpendingRecordId = spendingRecordId;
            DiscountCategoryId = discountCategoryId;
            DiscountValue= discountValue;
            Validate();
        }

        protected override void Validate()
        {
            IdValidation(Id);
            PositiveValueValidation(nameof(SpendingRecordId), SpendingRecordId);

            if(DiscountCategoryId != null)
                PositiveValueValidation(nameof(DiscountCategoryId), (int)DiscountCategoryId);

            PositiveValueValidation(nameof(DiscountValue), DiscountValue, true); 
        }
    }
}

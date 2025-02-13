using FinCtrlLibrary.Models;
using FinCtrlLibrary.Validators;

namespace FinCtrl.Test.Models.Constructors
{
    public class EarningRecordConstructor
    {
        [Fact]
        public void ReturnsValidEarningRecordWhenDataIsValid()
        {
            DateTime dateTime = DateTime.Now;
            string earningCategoryBsonId = "64c91f5e2b8f4e34a1d2cabc";
            double value = 100;
            bool validation = true;

            EarningRecord earningRecord = new(dateTime, earningCategoryBsonId, value);

            Assert.Equal(validation, earningRecord.IsValid);
        }

        [Fact]
        public void ReturnsValidEarningRecordWhenDateTimeIsNotInformed()
        {
            string earningCategoryBsonId = "64c91f5e2b8f4e34a1d2cabc";
            double value = 100;
            bool validation = true;

            EarningRecord earningRecord = new(earningCategoryBsonId, value);

            Assert.Equal(validation, earningRecord.IsValid);
        }

        [Theory]
        [InlineData("InvalidBsonId", 100, GenericErrors.InvalidBsonIdError, nameof(EarningRecord.EarningCategoryBsonId), false)]
        [InlineData("64c91f5e2b8f4e34a1d2cabc", 0, GenericErrors.ValueZeroError, nameof(EarningRecord.Value), false)]
        [InlineData("64c91f5e2b8f4e34a1d2cabc", -100, GenericErrors.NegativeValueError, nameof(EarningRecord.Value), false)]
        public void ReturnsErrorWhenPossibleValuesAreInvalid(string earningCategoryBsonId, double value, Enum error, string propertyName, bool validation)
        {
            EarningRecord earningRecord = new(earningCategoryBsonId, value);
            Assert.Equal(earningRecord.IsValid, validation);
            Assert.True(earningRecord.ContainsError(error, propertyName));
        }
    }
}

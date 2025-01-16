using FinCtrlLibrary.Models;
using FinCtrlLibrary.Utilities;
using FinCtrlLibrary.Validators;
using static FinCtrlLibrary.Models.SpendingRule;
using static FinCtrlLibrary.Utilities.WorkHoursAndSalary;

namespace FinCtrl.Test.Utilities.Constructors
{
    public class WorkHoursAndSalaryConstructor
    {
        [Theory]
        [InlineData(30, 24, 5000, true)]
        [InlineData(20, 8, 10000, true)]
        [InlineData(30, 1, 1000, true)]
        public void ReturnsValidWhenDataIsValid(int daysOfWorkPerMonth, int hoursOfWorkPerDay, double liquidSalary, bool validation)
        {
            WorkHoursAndSalary workHoursAndSalary = new(daysOfWorkPerMonth, hoursOfWorkPerDay, liquidSalary);

            Assert.Equal(workHoursAndSalary.IsValid, validation);
        }

        [Theory]
        [InlineData(31, 24, 5000, false)]
        [InlineData(20, 25, 10000, false)]
        [InlineData(200, 600, 1000, false)]
        [InlineData(20, 8, -8000, false)]
        public void ReturnsInvalidWhenDataIsInvalid(int daysOfWorkPerMonth, int hoursOfWorkPerDay, double liquidSalary, bool validation)
        {
            WorkHoursAndSalary workHoursAndSalary = new(daysOfWorkPerMonth, hoursOfWorkPerDay, liquidSalary);

            Assert.Equal(workHoursAndSalary.IsValid, validation);
        }

        [Theory]
        [InlineData(-30, 24, 5000, GenericErrors.NegativeValueError, nameof(WorkHoursAndSalary.DaysOfWorkPerMonth), false)]
        [InlineData(0, 24, 5000, GenericErrors.ValueZeroError, nameof(WorkHoursAndSalary.DaysOfWorkPerMonth), false)]
        [InlineData(30, -24, 5000, GenericErrors.NegativeValueError, nameof(WorkHoursAndSalary.HoursOfWorkPerDay), false)]
        [InlineData(20, 0, 5000, GenericErrors.ValueZeroError, nameof(WorkHoursAndSalary.HoursOfWorkPerDay), false)]
        [InlineData(20, 8, -10000, GenericErrors.NegativeValueError, nameof(WorkHoursAndSalary.LiquidSalaryInput), false)]
        [InlineData(20, 8, 0, GenericErrors.ValueZeroError, nameof(WorkHoursAndSalary.LiquidSalaryInput), false)]
        [InlineData(31, 8, 5000, WorkHoursAndSalaryCustomErrors.DaysExceedMaximumDaysPerMonth, nameof(WorkHoursAndSalary.DaysOfWorkPerMonth), false)]
        [InlineData(20, 25, 5000, WorkHoursAndSalaryCustomErrors.HoursExceedMaximumHoursPerDay, nameof(WorkHoursAndSalary.HoursOfWorkPerDay), false)]
        public void ReturnsErrorWhenPossibleValuesAreInvalid(int daysOfWorkPerMonth, int hoursOfWorkPerDay, double liquidSalary, Enum error, string propertyName, bool validation)
        {
            WorkHoursAndSalary workHoursAndSalary = new(daysOfWorkPerMonth, hoursOfWorkPerDay, liquidSalary);
            Assert.Equal(workHoursAndSalary.IsValid, validation);
            Assert.True(workHoursAndSalary.ContainsError(error, propertyName));
        }
    }
}
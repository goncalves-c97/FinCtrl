using FinCtrlLibrary.Utilities;
using FinCtrlLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FinCtrlLibrary.Utilities.WorkHoursAndSalary;

namespace FinCtrl.Test.Utilities.Constructors
{
    public class MoneyByTimeCalculatorConstructor
    {
        [Theory]
        [InlineData(30, 24, 5000, true)]
        [InlineData(20, 8, 10000, true)]
        [InlineData(30, 1, 1000, true)]
        public void ReturnsValidWhenDataIsValid(int workDaysPerMonth, int workHoursPerDay, double netSalary, bool validation)
        {
            WorkHoursAndSalary workHoursAndSalary = new(workDaysPerMonth, workHoursPerDay, netSalary);
            MoneyByTimeCalculator moneyByTimeCalculator = new(workHoursAndSalary);
            Assert.Equal(moneyByTimeCalculator.IsValid, validation);
        }

        [Theory]
        [InlineData(31, 24, 5000, false)]
        [InlineData(20, 25, 10000, false)]
        [InlineData(200, 600, 1000, false)]
        [InlineData(20, 8, -8000, false)]
        public void ReturnsInvalidWhenDataIsInvalid(int workDaysPerMonth, int workHoursPerDay, double netSalary, bool validation)
        {
            WorkHoursAndSalary workHoursAndSalary = new(workDaysPerMonth, workHoursPerDay, netSalary);
            MoneyByTimeCalculator moneyByTimeCalculator = new(workHoursAndSalary);
            Assert.Equal(moneyByTimeCalculator.IsValid, validation);
        }

        [Theory]
        [InlineData(-30, 24, 5000, GenericErrors.NegativeValueError, nameof(WorkHoursAndSalary.WorkDaysPerMonth), false)]
        [InlineData(0, 24, 5000, GenericErrors.ValueZeroError, nameof(WorkHoursAndSalary.WorkDaysPerMonth), false)]
        [InlineData(30, -24, 5000, GenericErrors.NegativeValueError, nameof(WorkHoursAndSalary.WorkHoursPerDay), false)]
        [InlineData(20, 0, 5000, GenericErrors.ValueZeroError, nameof(WorkHoursAndSalary.WorkHoursPerDay), false)]
        [InlineData(20, 8, -10000, GenericErrors.NegativeValueError, nameof(WorkHoursAndSalary.NetSalary), false)]
        [InlineData(20, 8, 0, GenericErrors.ValueZeroError, nameof(WorkHoursAndSalary.NetSalary), false)]
        [InlineData(31, 8, 5000, WorkHoursAndSalaryCustomErrors.ExceededMaximumDaysPerMonthError, nameof(WorkHoursAndSalary.WorkDaysPerMonth), false)]
        [InlineData(20, 25, 5000, WorkHoursAndSalaryCustomErrors.ExceededMaximumHoursPerDayError, nameof(WorkHoursAndSalary.WorkHoursPerDay), false)]
        public void ReturnsErrorWhenPossibleValuesAreInvalid(int workDaysPerMonth, int workHoursPerDay, double netSalary, Enum error, string propertyName, bool validation)
        {
            WorkHoursAndSalary workHoursAndSalary = new(workDaysPerMonth, workHoursPerDay, netSalary);
            MoneyByTimeCalculator moneyByTimeCalculator = new(workHoursAndSalary);
            Assert.Equal(moneyByTimeCalculator.IsValid, validation);
            Assert.True(moneyByTimeCalculator.ContainsError(error, propertyName));
        }
    }
}

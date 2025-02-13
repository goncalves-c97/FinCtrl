using FinCtrlLibrary.Models;
using FinCtrlLibrary.Utilities;
using FinCtrlLibrary.Validators;
using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCtrl.Test.Models.Constructors
{
    public class SalaryHistoryConstructor
    {
        [Theory]
        [InlineData("2010-01-01", "2024-12-31", 10000, true)]
        [InlineData("2024-12-01", "2024-12-31", 10000, true)]
        [InlineData("2024-12-01 00:00:00", "2024-12-01 23:59:59", 10000, true)]
        public void ReturnsValidSalaryHistoryWhenDataIsValid(DateTime startDateTime, DateTime endDateTime, double salary, bool validation)
        {
            SalaryHistory salaryHistory = new(startDateTime, endDateTime, salary);
            Assert.Equal(salaryHistory.IsValid, validation);
        }

        [Fact]
        public void ReturnsValidSalaryHistoryWhenNotSpecifyingEndDate()
        {
            DateTime startDateTime = new(2010, 01, 01);
            double salary = 10000;
            bool validation = true;

            SalaryHistory salaryHistory = new(startDateTime, salary);
            Assert.Equal(salaryHistory.IsValid, validation);
        }

        [Theory]
        [InlineData("2010-01-01", "2024-12-31", 0, false)]
        [InlineData("2010-01-01", "2024-12-31", -10000, false)]
        [InlineData("2010-01-01", "2010-01-01", 10000, false)]
        [InlineData("2024-12-31", "2024-12-01", 10000, false)]
        [InlineData("2024-12-01 23:59:59", "2024-12-01 00:00:00", 10000, false)]
        public void ReturnsInvalidSalaryHistoryWhenDataIsInvalid(DateTime startDateTime, DateTime endDateTime, double salary, bool validation)
        {
            SalaryHistory salaryHistory = new(startDateTime, endDateTime, salary);
            Assert.Equal(salaryHistory.IsValid, validation);
        }

        [Theory]
        [InlineData("2010-01-01", "2024-12-31", 0, GenericErrors.ValueZeroError, new string [] { nameof(SalaryHistory.Salary) }, false)]
        [InlineData("2010-01-01", "2024-12-31", -10000, GenericErrors.NegativeValueError, new string[] { nameof(SalaryHistory.Salary) }, false)]
        [InlineData("2010-01-01", "2010-01-01", 10000, GenericErrors.SameDateTimeError, new string[] { nameof(SalaryHistory.StartDate), nameof(SalaryHistory.EndDate) }, false)]
        [InlineData("2024-12-31", "2024-12-01", 10000, GenericErrors.StartBiggerThanEndDateTimeError, new string[] { nameof(SalaryHistory.StartDate), nameof(SalaryHistory.EndDate) }, false)]
        [InlineData("2024-12-01 23:59:59", "2024-12-01 00:00:00", 10000, GenericErrors.StartBiggerThanEndDateTimeError, new string[] { nameof(SalaryHistory.StartDate), nameof(SalaryHistory.EndDate) }, false)]
        public void ReturnsErrorWhenPossibleValuesAreInvalid(DateTime startDateTime, DateTime endDateTime, double salary, Enum error, string [] propertyNames, bool validation)
        {
            SalaryHistory salaryHistory = new SalaryHistory(startDateTime, endDateTime, salary);
            Assert.Equal(salaryHistory.IsValid, validation);
            Assert.True(salaryHistory.ContainsError(error, propertyNames));
        }
    }
}

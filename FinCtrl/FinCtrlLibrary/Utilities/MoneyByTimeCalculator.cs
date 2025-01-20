using FinCtrlLibrary.Validators;
using FinCtrlLibrary.Utilities;
using static FinCtrlLibrary.Utilities.ProjEnumerators;

namespace FinCtrlLibrary.Utilities
{
    public class MoneyByTimeCalculator : Validator
    {
        private double year;
        private double month;
        private double day;
        private double hour;
        private double minute;
        private double second;

        public WorkHoursAndSalary WorkHoursAndSalary { get; private set; }
        public double Value { get; private set; }
        public double Year { get => year; private set => CalculateByYearInputValue(value); }
        public double Month { get => month; private set => CalculateByMonthInputValue(value); }
        public double Day { get => day; private set => CalculateByDayInputValue(value); }
        public double Hour { get => hour; private set => CalculateByHourInputValue(value); }
        public double Minute { get => minute; private set => CalculateByMinuteInputValue(value); }
        public double Second { get => second; private set => CalculateBySecondInputValue(value); }

        public MoneyByTimeCalculator(WorkHoursAndSalary workHoursAndSalary)
        {
            WorkHoursAndSalary = workHoursAndSalary;
            UpdateInputValue(BaseTimePeriod.Month, WorkHoursAndSalary.NetSalary);
            Validate();
        }

        public void UpdateInputValue(BaseTimePeriod basePeriod, double value)
        {
            Value = value;

            switch (basePeriod)
            {
                case BaseTimePeriod.Year:
                    Year = value;
                    break;
                case BaseTimePeriod.Month:
                    Month = value;
                    break;
                case BaseTimePeriod.Day:
                    Day = value;
                    break;
                case BaseTimePeriod.Hour:
                    Hour = value;
                    break;
                case BaseTimePeriod.Minute:
                    Minute = value;
                    break;
                case BaseTimePeriod.Second:
                    Second = value;
                    break;
            }
        }

        private void CalculateByYearInputValue(double value)
        {
            year = value;

            month = Year / 12;
            day = Month / WorkHoursAndSalary.WorkDaysPerMonth;
            hour = Day / WorkHoursAndSalary.WorkHoursPerDay;
            minute = Hour / 60;
            second = Minute / 60;
        }

        private void CalculateByMonthInputValue(double value)
        {
            month = value;

            year = Month * 12;

            day = Month / WorkHoursAndSalary.WorkDaysPerMonth;
            hour = Day / WorkHoursAndSalary.WorkHoursPerDay;
            minute = Hour / 60;
            second = Minute / 60;
        }

        private void CalculateByDayInputValue(double value)
        {
            day = value;

            month = WorkHoursAndSalary.WorkDaysPerMonth * Day;
            year = Month * 12;

            hour = Day / WorkHoursAndSalary.WorkHoursPerDay;
            minute = Hour / 60;
            second = Minute / 60;
        }

        private void CalculateByHourInputValue(double value)
        {
            hour = value;

            day = WorkHoursAndSalary.WorkHoursPerDay * Hour;
            month = WorkHoursAndSalary.WorkDaysPerMonth * Day;
            year = Month * 12;

            minute = Hour / 60;
            second = Minute / 60;
        }

        private void CalculateByMinuteInputValue(double value)
        {
            minute = value;

            hour = Minute * 60;
            day = WorkHoursAndSalary.WorkHoursPerDay * Hour;
            month = WorkHoursAndSalary.WorkDaysPerMonth * Day;
            year = Month * 12;

            second = Minute / 60;
        }

        private void CalculateBySecondInputValue(double value)
        {
            second = value;

            minute = Second * 60;
            hour = Minute * 60;
            day = WorkHoursAndSalary.WorkHoursPerDay * Hour;
            month = WorkHoursAndSalary.WorkDaysPerMonth * Day;
            year = Month * 12;
        }

        protected override void Validate()
        {
            NotNullValueValidation(WorkHoursAndSalary, nameof(WorkHoursAndSalary));
            ValidObjectValidation(WorkHoursAndSalary, nameof(WorkHoursAndSalary));
        }
    }
}

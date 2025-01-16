using FinCtrlLibrary.Validators;

namespace FinCtrlLibrary.Utilities
{
    public enum BasePeriod
    {
        Year,
        Month,
        Day,
        Hour,
        Minute,
        Second
    }

    public class MoneyPerPeriodCalculator : Validator
    {
        public WorkHoursAndSalary WorkHoursAndSalary { get; set; }
        public double Year { get; set; }
        public double Month { get; set; }
        public double Day { get; set; }
        public double Hour { get; set; }
        public double Minute { get; set; }
        public double Second { get; set; }

        public MoneyPerPeriodCalculator(WorkHoursAndSalary workHoursAndSalary, BasePeriod basePeriod, double value)
        {
            WorkHoursAndSalary = workHoursAndSalary;

            switch (basePeriod)
            {
                case BasePeriod.Year:
                    Year = value;
                    break;
                case BasePeriod.Month:
                    Month = value;
                    break;
                case BasePeriod.Day:
                    Day = value;
                    break;
                case BasePeriod.Hour:
                    Hour = value;
                    break;
                case BasePeriod.Minute:
                    Minute = value;
                    break;
                case BasePeriod.Second:
                    Second = value;
                    break;
            }

            if (basePeriod == BasePeriod.Year)
            {
                Month = Year / 12;
                Day = Month / workHoursAndSalary.DaysOfWorkPerMonth;
                Hour = Day / workHoursAndSalary.HoursOfWorkPerDay;
                Minute = Hour / 60;
                Second = Minute / 60;
            }
            else if (basePeriod == BasePeriod.Month)
            {
                Year = Month * 12;
                
                Day = Month / workHoursAndSalary.DaysOfWorkPerMonth;
                Hour = Day / workHoursAndSalary.HoursOfWorkPerDay;
                Minute = Hour / 60;
                Second = Minute / 60;
            }
            else if (basePeriod == BasePeriod.Day)
            {
                Month = workHoursAndSalary.DaysOfWorkPerMonth * Day;
                Year = Month * 12;

                Hour = Day / workHoursAndSalary.HoursOfWorkPerDay;
                Minute = Hour / 60;
                Second = Minute / 60;
            }
            else if (basePeriod == BasePeriod.Hour)
            {
                Day = workHoursAndSalary.HoursOfWorkPerDay * Hour;
                Month = workHoursAndSalary.DaysOfWorkPerMonth * Day;
                Year = Month * 12;

                Minute = Hour / 60;
                Second = Minute / 60;
            }
            else if (basePeriod == BasePeriod.Minute)
            {
                Hour = Minute * 60;
                Day = workHoursAndSalary.HoursOfWorkPerDay * Hour;
                Month = workHoursAndSalary.DaysOfWorkPerMonth * Day;
                Year = Month * 12;

                Second = Minute / 60;
            }
            else if (basePeriod == BasePeriod.Second)
            {
                Minute = Second * 60;
                Hour = Minute * 60;
                Day = workHoursAndSalary.HoursOfWorkPerDay * Hour;
                Month = workHoursAndSalary.DaysOfWorkPerMonth * Day;
                Year = Month * 12;
            }
        }

        protected override void Validate()
        {
            
        }
    }
}

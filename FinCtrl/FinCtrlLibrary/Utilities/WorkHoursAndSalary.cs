using FinCtrlLibrary.Validators;

namespace FinCtrlLibrary.Utilities
{
    public class WorkHoursAndSalary : Validator
    {
        private const int _maximumDaysOfWorkPerMonth = 30;
        private const int _maximumHoursOfWorkPerDay = 24;

        public int DaysOfWorkPerMonth { get; set; }
        public int HoursOfWorkPerDay { get; set; }
        public double LiquidSalaryInput { get; set; }

        public WorkHoursAndSalary(int daysOfWorkPerMonth, int hoursOfWorkPerDay, double liquidSalaryInput)
        {
            DaysOfWorkPerMonth = daysOfWorkPerMonth;
            HoursOfWorkPerDay = hoursOfWorkPerDay;
            LiquidSalaryInput = liquidSalaryInput;
            Validate();
        }

        public enum WorkHoursAndSalaryCustomErrors
        {
            DaysExceedMaximumDaysPerMonth,
            HoursExceedMaximumHoursPerDay
        }

        public static WorkHoursAndSalary GetFullTimeConfiguration(int liquidSalary)
        {
            return new(_maximumDaysOfWorkPerMonth, _maximumHoursOfWorkPerDay, liquidSalary);
        }

        protected override void Validate()
        {
            PositiveValueValidation(nameof(DaysOfWorkPerMonth), DaysOfWorkPerMonth, true);
            PositiveValueValidation(nameof(HoursOfWorkPerDay), HoursOfWorkPerDay, true);
            PositiveValueValidation(nameof(LiquidSalaryInput), LiquidSalaryInput, true);

            if (DaysOfWorkPerMonth > _maximumDaysOfWorkPerMonth)
                Errors.RegisterError(WorkHoursAndSalaryCustomErrors.DaysExceedMaximumDaysPerMonth, $"Os dias de trabalho por mês não podem exceder {_maximumDaysOfWorkPerMonth} dias.", nameof(DaysOfWorkPerMonth));

            if (HoursOfWorkPerDay > _maximumHoursOfWorkPerDay)
                Errors.RegisterError(WorkHoursAndSalaryCustomErrors.HoursExceedMaximumHoursPerDay, $"As horas de trabalho por dia não podem exceder {_maximumHoursOfWorkPerDay} horas.", nameof(HoursOfWorkPerDay));
        }
    }
}

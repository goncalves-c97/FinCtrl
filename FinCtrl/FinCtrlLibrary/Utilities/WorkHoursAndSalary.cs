using FinCtrlLibrary.Validators;
using static FinCtrlLibrary.Utilities.ProjEnumerators;

namespace FinCtrlLibrary.Utilities
{
    public class WorkHoursAndSalary : Validator
    {
        private const int _maximumWorkDaysPerMonth = 30;
        private const int _maximumWorkHoursPerDay = 24;

        public int WorkDaysPerMonth { get; set; }
        public int WorkHoursPerDay { get; set; }
        public double NetSalary { get; set; }

        public WorkHoursAndSalary(int workDaysPerMonth, int workHoursPerDay, double netSalary)
        {
            WorkDaysPerMonth = workDaysPerMonth;
            WorkHoursPerDay = workHoursPerDay;
            NetSalary = netSalary;
            Validate();
        }

        public enum WorkHoursAndSalaryCustomErrors
        {
            ExceededMaximumDaysPerMonthError,
            ExceededMaximumHoursPerDayError
        }

        public string CalculateWorkTimeForPayment(double paidValue)
        {
            double annualEarnings = NetSalary * 12;
            double years = Math.Floor(paidValue / annualEarnings);
            double remainingAfterYears = paidValue % annualEarnings;

            double months = Math.Floor(remainingAfterYears / NetSalary);
            double remainingAfterMonths = remainingAfterYears % NetSalary;

            double dailyEarnings = NetSalary / WorkDaysPerMonth;
            double days = Math.Floor(remainingAfterMonths / dailyEarnings);
            double remainingAfterDays = remainingAfterMonths % dailyEarnings;

            double hourlyEarnings = dailyEarnings / WorkHoursPerDay;
            double hours = Math.Floor(remainingAfterDays / hourlyEarnings);
            double remainingAfterHours = remainingAfterDays % hourlyEarnings;

            double minutes = Math.Floor(remainingAfterHours / (hourlyEarnings / 60));
            double remainingAfterMinutes = remainingAfterHours % (hourlyEarnings / 60);

            double seconds = Math.Floor(remainingAfterMinutes * 60);

            List<string> paymentTimeItems =
            [
                GetBaseTimePeriodStringByValue(BaseTimePeriod.Year, years),
                GetBaseTimePeriodStringByValue(BaseTimePeriod.Month, months),
                GetBaseTimePeriodStringByValue(BaseTimePeriod.Day, days),
                GetBaseTimePeriodStringByValue(BaseTimePeriod.Hour, hours),
                GetBaseTimePeriodStringByValue(BaseTimePeriod.Minute, minutes),
                GetBaseTimePeriodStringByValue(BaseTimePeriod.Second, seconds)
            ];

            // Filtragem dos valores de tempo não vazios
            paymentTimeItems = paymentTimeItems
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (paymentTimeItems.Count == 0)
                return string.Empty;
            else if(paymentTimeItems.Count == 1)
                return paymentTimeItems[0];
            else
            {
                string result = $"e {paymentTimeItems[^1]}";

                for (int i = paymentTimeItems.Count - 2; i >= 0; i--)
                    result = $"{paymentTimeItems[i]}, {result}";

                return result;
            }
        }

        public string CalculateSalaryPercentageForPayment(double paidValue)
        {
            double percentage = Math.Round((paidValue / NetSalary) * 100, 2);
            return $"{percentage} %";
        }

        private static string GetBaseTimePeriodStringByValue(BaseTimePeriod baseTimePeriod, double value)
        {
            bool zero = value <= 0;

            if (zero)
                return string.Empty;

            bool plural = value > 1;

            switch (baseTimePeriod)
            {
                case BaseTimePeriod.Year:
                    return $"{value} " + (plural ? "ANOS" : "ANO");
                case BaseTimePeriod.Month:
                    return $"{value} " + (plural ? "MÊS" : "MESES");
                case BaseTimePeriod.Day:
                    return $"{value} " + (plural ? "DIAS" : "DIA");
                case BaseTimePeriod.Hour:
                    return $"{value} " + (plural ? "HORAS" : "HORA");
                case BaseTimePeriod.Minute:
                    return $"{value} " + (plural ? "MINUTOS" : "MINUTO");
                case BaseTimePeriod.Second:
                    return $"{value} " + (plural ? "SEGUNDOS" : "SEGUNDO");
                default:
                    return string.Empty;
            }
        }

        public static WorkHoursAndSalary GetFullTimeConfiguration(int liquidSalary)
        {
            return new(_maximumWorkDaysPerMonth, _maximumWorkHoursPerDay, liquidSalary);
        }

        protected override void Validate()
        {
            PositiveValueValidation(nameof(WorkDaysPerMonth), WorkDaysPerMonth, true);
            PositiveValueValidation(nameof(WorkHoursPerDay), WorkHoursPerDay, true);
            PositiveValueValidation(nameof(NetSalary), NetSalary, true);

            if (WorkDaysPerMonth > _maximumWorkDaysPerMonth)
                Errors.RegisterError(WorkHoursAndSalaryCustomErrors.ExceededMaximumDaysPerMonthError, $"Os dias de trabalho por mês não podem exceder {_maximumWorkDaysPerMonth} dias.", nameof(WorkDaysPerMonth));

            if (WorkHoursPerDay > _maximumWorkHoursPerDay)
                Errors.RegisterError(WorkHoursAndSalaryCustomErrors.ExceededMaximumHoursPerDayError, $"As horas de trabalho por dia não podem exceder {_maximumWorkHoursPerDay} horas.", nameof(WorkHoursPerDay));
        }
    }
}

using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinCtrlLibrary.Models
{
    public class SalaryHistory : ValidatorClass, IMongoDbItem
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double Salary { get; set; }

        public SalaryHistory()
        {
            Validate();
        }

        public SalaryHistory(DateTime startDate, double salary)
        {
            StartDate = startDate;
            EndDate = null;
            Salary = salary;
            Validate();
        }

        public SalaryHistory(DateTime startDate, DateTime endDate, double salary)
        {
            StartDate = startDate;
            EndDate = endDate;
            Salary = salary;
            Validate();
        }

        protected override void Validate()
        {
            if (EndDate != null)
                StartEndDateTimeValidation(StartDate, nameof(StartDate), (DateTime)EndDate, nameof(EndDate), true);

            PositiveValueValidation(nameof(Salary), Salary, true);
        }
    }
}

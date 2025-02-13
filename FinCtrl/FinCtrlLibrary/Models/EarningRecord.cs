using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace FinCtrlLibrary.Models
{
    public class EarningRecord : ValidatorClass, IMongoDbItem
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public DateTime DateTime { get; set; }
        [JsonIgnore, BsonIgnore]
        public int EarningCategoryId { get; set; }
        public string EarningCategoryBsonId { get; set; }
        public string? Observation { get; set; }
        public double Value { get; set; }

        public EarningCategory EarningCategory { get; set; }

        public EarningRecord() 
        {
            Validate();
        }

        public EarningRecord(string earningCategoryBsonId, double value)
        {
            DateTime = DateTime.Now;
            EarningCategoryBsonId = earningCategoryBsonId;
            Value = value;
            Validate();
        }

        public EarningRecord(DateTime dateTime, string earningCategoryBsonId, double value)
        {
            DateTime = dateTime;
            EarningCategoryBsonId = earningCategoryBsonId;
            Value = value;
            Validate();
        }

        public override bool Equals(object? obj)
        {
            bool isEarningRecord = obj is EarningRecord;

            if (!isEarningRecord)
                return false;

            EarningRecord earningRecord = (obj as EarningRecord)!;

            bool sameDate = DateTime.ToShortDateString() == earningRecord.DateTime.ToShortDateString();

            bool sameCategory = EarningCategoryBsonId.Equals(earningRecord.EarningCategoryBsonId, StringComparison.InvariantCultureIgnoreCase);

            bool nullObservations = Observation == null && earningRecord.Observation == null;

            bool differentNullStateObservations = Observation == null || earningRecord.Observation == null;

            if (nullObservations)
                return sameDate && sameCategory;
            else if (differentNullStateObservations)
                return false;
            else
            {
                bool equalObservations = Observation!.Equals(earningRecord.Observation, StringComparison.InvariantCultureIgnoreCase);
                return sameDate && sameCategory && equalObservations;
            }
        }

        protected override void Validate()
        {
            PositiveValueValidation(nameof(Value), Value, true);
            ObjectIdValidation(nameof(EarningCategoryBsonId), EarningCategoryBsonId);
        }
    }
}

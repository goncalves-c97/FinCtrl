using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace FinCtrlLibrary.Models
{
    public class EarningsRecord : ValidatorClass, IMongoDbItem
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public DateTime DateTime { get; set; }
        [JsonIgnore, BsonIgnore]
        public int EarningCategoryId { get; set; }
        public string EarningCategoryBsonId { get; set; }
        public double Value { get; set; }

        public EarningCategory EarningGategory { get; set; }

        public EarningsRecord() 
        {
            Validate();
        }

        public EarningsRecord(string categoryId, double value)
        {
            DateTime = DateTime.Now;
            EarningCategoryBsonId = categoryId;
            Value = value;
            Validate();
        }

        public EarningsRecord(DateTime dateTime, string categoryId, double value)
        {
            DateTime = dateTime;
            EarningCategoryBsonId = categoryId;
            Value = value;
            Validate();
        }

        protected override void Validate()
        {
            PositiveValueValidation(nameof(Value), Value, true);
            ObjectIdValidation(nameof(EarningCategoryBsonId), EarningCategoryBsonId);
        }
    }
}

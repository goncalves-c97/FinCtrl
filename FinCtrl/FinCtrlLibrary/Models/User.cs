using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinCtrlLibrary.Models
{
    public class User : ValidatorClass, IMongoDbItem
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastLogin { get; set; }

        protected override void Validate()
        {
            ObjectIdValidation(nameof(_id), _id.ToString());
            NotEmptyStringValidation(nameof(Name), Name);
            NotEmptyStringValidation(nameof(Email), Email);
            StartEndDateTimeValidation(CreationDateTime, nameof(CreationDateTime), DateTime.Now, nameof(DateTime.Now));
            NotEmptyStringValidation(nameof(Password), Password);
            NotEmptyStringValidation(nameof(Role), Role);
            NotBiggerThanNowDateTimeValidation(nameof(DateOfBirth), DateOfBirth);
            NotBiggerThanNowDateTimeValidation(nameof(LastLogin), LastLogin);
        }
    }
}

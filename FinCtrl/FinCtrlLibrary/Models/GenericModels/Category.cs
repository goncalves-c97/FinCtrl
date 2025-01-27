using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinCtrlLibrary.Models.GenericModels
{
    public class Category : ValidatorClass, IMongoDbItem
    {
        private const int _nameMaxLength = 30;

        [BsonId]
        public ObjectId _id { get; set; }
        [BsonIgnore, BsonElement("id")]
        public int Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }

        public Category()
        {

        }

        public Category(string name)
        {
            Id = 0;
            Name = name;
            Validate();
        }

        public Category(string bsonId, string name)
        {
            Id = 0;
            _id = ObjectId.Parse(bsonId);
            Name = name;
            Validate();
        }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
            Validate();
        }

        protected override void Validate()
        {
            IdValidation(Id, nameof(Id));
            NotEmptyStringValidation(nameof(Name), Name);
            NotEmptyStringLengthValidation(nameof(Name), Name, _nameMaxLength);
        }

        public override string ToString()
        {
            return $"{_id} - {Name}";
        }
    }
}

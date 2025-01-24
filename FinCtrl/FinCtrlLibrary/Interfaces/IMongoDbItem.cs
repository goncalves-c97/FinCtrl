using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FinCtrlLibrary.Interfaces
{
    public interface IMongoDbItem
    {
        [BsonId]
        public ObjectId _id { get; set; }
    }
}

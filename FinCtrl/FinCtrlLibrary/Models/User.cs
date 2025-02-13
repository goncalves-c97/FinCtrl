using FinCtrlLibrary.Interfaces;
using MongoDB.Bson;

namespace FinCtrlLibrary.Models
{
    public class User : IMongoDbItem
    {
        public ObjectId _id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string Password { get; set; }
    }
}

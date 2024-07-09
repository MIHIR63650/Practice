using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApiProject.Models
{
    public class MyItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string ItemName { get; set; }

        public decimal Price { get; set; }
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoAuthApi.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string User { get; set; } = null!;
        public string Message { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
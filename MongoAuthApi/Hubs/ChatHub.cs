using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using MongoAuthApi.Models;

namespace MongoAuthApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<ChatMessage> _chatCollection;

        // Inject MongoDB Database
        public ChatHub(IMongoDatabase database)
        {
            _chatCollection = database.GetCollection<ChatMessage>("ChatMessages");
        }

        public async Task SendMessage(string user, string message)
        {
            
            var chatLog = new ChatMessage
            {
                User = user,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            // Save to MongoDB
            await _chatCollection.InsertOneAsync(chatLog);

            // Broadcast
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
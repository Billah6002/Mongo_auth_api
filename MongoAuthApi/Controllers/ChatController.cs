using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoAuthApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace MongoAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMongoCollection<ChatMessage> _chatCollection;

        public ChatController(IMongoDatabase database)
        {
            _chatCollection = database.GetCollection<ChatMessage>("ChatMessages");
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            // Get the last 50 messages, sorted by time
            var messages = await _chatCollection.Find(_ => true)
                .SortBy(m => m.Timestamp)
                .Limit(50)
                .ToListAsync();

            return Ok(messages);
        }
    }
}
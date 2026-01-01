namespace MongoAuthApi.Contracts
{
    public record UserRegisteredEvent
    {
        public string Email { get; init; } = null!;
        public string Username { get; init; } = null!;
    }
}
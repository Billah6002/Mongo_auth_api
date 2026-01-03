namespace MongoAuthApi.Settings;

public class RabbitMqSettings
{
    public required string Host { get; set; }
    public required string Username { get; set; } = "guest";
    public required string Password { get; set; } = "guest";
}

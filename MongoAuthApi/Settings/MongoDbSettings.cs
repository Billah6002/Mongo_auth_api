namespace MongoAuthApi.Settings;

public class MongoDbSettings
{
    public required string ConnectionURI { get; set; }
    public required string DatabaseName { get; set; }
    public required string CollectionName { get; set; }
}

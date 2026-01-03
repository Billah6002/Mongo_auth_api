namespace MongoAuthApi.Settings;

public class EmailSettings
{
    public required string SenderEmail { get; set; }
    public required string AppPassword { get; set; }
    public required string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
}

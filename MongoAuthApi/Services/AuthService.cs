using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoAuthApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MassTransit;
using MongoAuthApi.Contracts;

public class AuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly IConfiguration _config;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthService(IConfiguration config, IPublishEndpoint publishEndpoint)
    {
        _config = config;
        _publishEndpoint = publishEndpoint;
        var mongoClient = new MongoClient(_config["MongoDB:ConnectionURI"]);
        var mongoDatabase = mongoClient.GetDatabase(_config["MongoDB:DatabaseName"]);
        _users = mongoDatabase.GetCollection<User>(_config["MongoDB:CollectionName"]);
    }

    public async Task RegisterAsync(string username, string email, string password)
    {
        var existingUser = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (existingUser != null) throw new Exception("User already exists");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        await _users.InsertOneAsync(user);
        // send the massage to rabbitmq
        await _publishEndpoint.Publish(new UserRegisteredEvent
        {
            Email = email,
            Username = username
        });
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null; // Invalid credentials

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
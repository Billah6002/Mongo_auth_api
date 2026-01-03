using System.ComponentModel.DataAnnotations;

namespace MongoAuthApi.Dtos;

public record UserDto(
    [Required] string Username, 
    [Required] [EmailAddress] string Email, 
    [Required] [MinLength(6)] string Password
);

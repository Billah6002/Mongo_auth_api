using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto request)
    {
        await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);
        if (token == null) return Unauthorized(new { message = "Invalid credentials" });

        return Ok(new { token });
    }
}

// Simple DTOs (Data Transfer Objects)
public record UserDto(
    [Required] string Username, 
    [Required] [EmailAddress] string Email, 
    [Required] [MinLength(6)] string Password
);

public record LoginDto(
    [Required] [EmailAddress] string Email, 
    [Required] string Password
);